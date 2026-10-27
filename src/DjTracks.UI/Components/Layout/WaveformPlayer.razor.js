const instances = new WeakMap();

export function clearWaveform(canvas) {
    const instance = instances.get(canvas);
    if (!instance) return;
    cancelAnimationFrame(instance.frame);
    instance.frame = 0;
    instance.peaks = [];
    instance.loadedCount = 0;
    draw(instance);
}

export function appendWaveformChunk(canvas, startIndex, newPeaks, peakCount, duration) {
    const audio = document.getElementById("djtracks-audio");
    if (!(audio instanceof HTMLAudioElement))
        throw new Error("No se encontró el reproductor de audio para sincronizar la onda.");
    let instance = instances.get(canvas);
    if (!instance) {
        instance = {
            canvas, audio, peaks: new Array(peakCount).fill(0),
            loadedCount: 0, duration, frame: 0
        };
        instance.draw = () => draw(instance);
        instance.seek = event => {
            const bounds = canvas.getBoundingClientRect();
            const ratio = Math.min(1, Math.max(0, (event.clientX - bounds.left) / bounds.width));
            seek(instance, ratio);
        };
        instance.keydown = event => {
            if (!["ArrowLeft", "ArrowRight", "Home", "End"].includes(event.key)) return;
            event.preventDefault();
            const duration = Number.isFinite(instance.audio.duration) && instance.audio.duration > 0
                ? instance.audio.duration : instance.duration;
            const current = duration > 0 ? instance.audio.currentTime / duration : 0;
            const step = duration > 0 ? Math.min(5 / duration, 1) : 0.01;
            const next = event.key === "Home" ? 0
                : event.key === "End" ? 1
                    : current + (event.key === "ArrowRight" ? step : -step);
            seek(instance, next);
        };
        instance.animate = () => {
            draw(instance);
            if (!instance.audio.paused && !instance.audio.ended)
                instance.frame = requestAnimationFrame(instance.animate);
        };
        canvas.addEventListener("click", instance.seek);
        canvas.addEventListener("keydown", instance.keydown);
        audio.addEventListener("timeupdate", instance.draw);
        audio.addEventListener("loadedmetadata", instance.draw);
        audio.addEventListener("seeked", instance.draw);
        audio.addEventListener("play", instance.animate);
        audio.addEventListener("pause", instance.draw);
        instance.resize = new ResizeObserver(instance.draw);
        instance.resize.observe(canvas);
        instances.set(canvas, instance);
    } else if (instance.peaks.length !== peakCount) {
        instance.peaks = new Array(peakCount).fill(0);
        instance.loadedCount = 0;
    }

    if (instance.audio !== audio) {
        detach(instance);
        instance.audio = audio;
        attachAudio(instance);
    }

    instance.duration = duration;
    for (let index = 0; index < newPeaks.length; index++)
        instance.peaks[startIndex + index] = newPeaks[index];
    instance.loadedCount = Math.max(instance.loadedCount, startIndex + newPeaks.length);
    draw(instance);
}

export function disposeWaveform(canvas) {
    const instance = instances.get(canvas);
    if (!instance) return;
    detach(instance);
    canvas.removeEventListener("click", instance.seek);
    canvas.removeEventListener("keydown", instance.keydown);
    instance.resize.disconnect();
    cancelAnimationFrame(instance.frame);
    instances.delete(canvas);
}

function attachAudio(instance) {
    instance.audio.addEventListener("timeupdate", instance.draw);
    instance.audio.addEventListener("loadedmetadata", instance.draw);
    instance.audio.addEventListener("seeked", instance.draw);
    instance.audio.addEventListener("play", instance.animate);
    instance.audio.addEventListener("pause", instance.draw);
}

function detach(instance) {
    instance.audio.removeEventListener("timeupdate", instance.draw);
    instance.audio.removeEventListener("loadedmetadata", instance.draw);
    instance.audio.removeEventListener("seeked", instance.draw);
    instance.audio.removeEventListener("play", instance.animate);
    instance.audio.removeEventListener("pause", instance.draw);
}

function seek(instance, ratio) {
    const audio = instance.audio;
    const duration = Number.isFinite(audio.duration) && audio.duration > 0
        ? audio.duration : instance.duration;
    if (duration > 0) {
        audio.currentTime = Math.min(1, Math.max(0, ratio)) * duration;
        draw(instance);
    }
}

function draw(instance) {
    const { canvas, audio, peaks } = instance;
    const width = Math.max(1, Math.floor(canvas.clientWidth));
    const height = Math.max(1, Math.floor(canvas.clientHeight));
    const scale = window.devicePixelRatio || 1;
    if (canvas.width !== width * scale || canvas.height !== height * scale) {
        canvas.width = width * scale;
        canvas.height = height * scale;
    }

    const context = canvas.getContext("2d");
    context.setTransform(scale, 0, 0, scale, 0, 0);
    context.clearRect(0, 0, width, height);
    if (peaks.length === 0) return;
    const style = getComputedStyle(canvas);
    const primary = style.getPropertyValue("--mud-palette-primary").trim() || style.color;
    const muted = style.getPropertyValue("--mud-palette-text-secondary").trim() || style.color;
    const duration = Number.isFinite(audio.duration) && audio.duration > 0
        ? audio.duration : instance.duration;
    const progress = duration > 0 ? audio.currentTime / duration : 0;
    const middle = height / 2;
    const barWidth = Math.max(1, width / peaks.length);
    const maxPeak = Math.max(0, ...peaks);

    peaks.slice(0, instance.loadedCount).forEach((peak, index) => {
        const x = index * width / peaks.length;
        const normalizedPeak = maxPeak > 0 ? Math.sqrt(peak / maxPeak) : 0;
        const amplitude = Math.max(1, normalizedPeak * (height * 0.88) / 2);
        context.strokeStyle = x / width <= progress ? primary : muted;
        context.lineWidth = barWidth;
        context.beginPath();
        context.moveTo(x, middle - amplitude);
        context.lineTo(x, middle + amplitude);
        context.stroke();
    });

    canvas.setAttribute("aria-valuenow", Math.round(progress * 100).toString());
    canvas.setAttribute("aria-valuetext",
        `${Math.floor(audio.currentTime / 60)}:${Math.floor(audio.currentTime % 60).toString().padStart(2, "0")}`);
}
