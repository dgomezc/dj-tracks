const players = new WeakMap();

export function observePlayer(root) {
    const player = { root, audio: null, listeners: [], error: null };
    player.resize = new ResizeObserver(() =>
        document.documentElement.style.setProperty("--player-height", `${root.offsetHeight}px`));
    player.resize.observe(root);
    root.querySelector(".player-volume-thumb")?.setAttribute("aria-label", "Volumen");
    players.set(root, player);
}

export function attachPlayer(root, audio, reference, version, volume, muted, playbackRecord) {
    const player = players.get(root);
    detachAudio(player);
    player.audio = audio;
    player.reference = reference;
    player.version = version;
    player.playbackRecord = playbackRecord;
    player.listenedSeconds = 0;
    player.playbackReported = false;
    player.trackingPlayback = false;
    player.lastMediaTime = null;
    player.lastSampleTime = null;
    player.error = null;
    updateTime(player);
    if (!audio) return;
    audio.volume = volume;
    audio.muted = muted;
    const listen = (target, event, handler) => {
        target.addEventListener(event, handler);
        player.listeners.push(() => target.removeEventListener(event, handler));
    };
    for (const event of ["timeupdate", "loadedmetadata", "durationchange", "seeked"])
        listen(audio, event, () => updateTime(player));
    listen(audio, "playing", () => {
        player.error = null;
        player.trackingPlayback = true;
        resetPlaybackSample(player);
        notify(player);
    });
    listen(audio, "timeupdate", () => samplePlayback(player));
    listen(audio, "pause", () => {
        samplePlayback(player, true);
        player.trackingPlayback = false;
        notify(player);
    });
    listen(audio, "waiting", () => {
        samplePlayback(player, true);
        player.trackingPlayback = false;
    });
    listen(audio, "seeking", () => {
        player.trackingPlayback = false;
        resetPlaybackSample(player);
    });
    listen(audio, "seeked", () => {
        resetPlaybackSample(player);
        player.trackingPlayback = !audio.paused;
        updateTime(player);
    });
    listen(audio, "ended", () => {
        samplePlayback(player, true);
        player.trackingPlayback = false;
        notify(player);
        invoke(player, "OnPlaybackEnded", version);
    });
    listen(audio, "error", () => {
        player.error = "No se pudo reproducir el audio. Comprueba el archivo y la API; vuelve a pulsar Reproducir.";
        notify(player);
    });
    const seek = root.querySelector("[data-player-seek]");
    if (seek)
        listen(seek, "input", () => {
            if (Number.isFinite(audio.duration) && audio.duration > 0)
                audio.currentTime = Number(seek.value) * audio.duration / 100;
        });
    startPlayback(player);
}

export function togglePlayback(root) {
    const player = players.get(root);
    if (!player?.audio) return;
    if (player.audio.paused) startPlayback(player);
    else player.audio.pause();
}

export function setVolume(root, volume, muted) {
    const audio = players.get(root)?.audio;
    if (!audio) return;
    audio.volume = volume;
    audio.muted = muted;
}

export function disposePlayer(root) {
    const player = players.get(root);
    if (!player) return;
    detachAudio(player);
    player.resize.disconnect();
    players.delete(root);
    document.documentElement.style.removeProperty("--player-height");
}

function detachAudio(player) {
    for (const remove of player.listeners) remove();
    player.listeners = [];
    if (player.audio) {
        player.audio.pause();
        player.audio.removeAttribute("src");
        player.audio.load();
    }
}

function startPlayback(player) {
    const audio = player.audio;
    player.error = null;
    audio.play().catch(error => {
        if (player.audio !== audio) return;
        player.error = error.name === "NotAllowedError"
            ? "El navegador ha bloqueado el inicio automático. Pulsa Reproducir para escuchar."
            : "No se pudo iniciar el audio. Comprueba el archivo y la API; vuelve a pulsar Reproducir.";
        console.warn("DJ Tracks: no se pudo iniciar el audio.", error);
        notify(player);
    });
}

function invoke(player, method, ...args) {
    player.reference.invokeMethodAsync(method, ...args)
        .catch(error => console.error("DJ Tracks: no se pudo notificar el estado del audio.", error));
}

function notify(player) {
    invoke(player, "OnPlaybackState", player.version, player.audio.paused, player.error);
}

function resetPlaybackSample(player) {
    player.lastMediaTime = player.audio?.currentTime ?? null;
    player.lastSampleTime = performance.now();
}

function samplePlayback(player, includeFinalProgress = false) {
    const audio = player.audio;
    const now = performance.now();
    const mediaTime = audio?.currentTime ?? 0;
    if (player.trackingPlayback && (!audio.paused || includeFinalProgress)
        && !audio.seeking && player.lastMediaTime !== null && player.lastSampleTime !== null) {
        const mediaDelta = mediaTime - player.lastMediaTime;
        const wallDelta = (now - player.lastSampleTime) / 1000;
        if (mediaDelta > 0 && mediaDelta <= wallDelta + 0.25)
            player.listenedSeconds += mediaDelta;
    }
    player.lastMediaTime = mediaTime;
    player.lastSampleTime = now;
    if (!player.playbackReported && player.listenedSeconds >= 30 && player.playbackRecord) {
        player.playbackReported = true;
        invoke(player, "OnPlaybackThreshold", player.version, player.playbackRecord);
    }
}

function formatTime(seconds) {
    const total = Math.max(0, Math.floor(seconds));
    const minutes = Math.floor(total / 60);
    return minutes >= 60
        ? `${Math.floor(minutes / 60)}:${String(minutes % 60).padStart(2, "0")}:${String(total % 60).padStart(2, "0")}`
        : `${minutes}:${String(total % 60).padStart(2, "0")}`;
}

function updateTime(player) {
    const { root, audio } = player;
    const duration = audio && Number.isFinite(audio.duration) ? audio.duration : 0;
    const elapsed = audio?.currentTime || 0;
    root.querySelector("[data-player-elapsed]").textContent = formatTime(elapsed);
    root.querySelector("[data-player-duration]").textContent = duration > 0 ? formatTime(duration) : "—";
    root.querySelector("[data-player-remaining]").textContent = duration > 0 ? `−${formatTime(duration - elapsed)}` : "—";
    root.querySelectorAll("[data-player-tick]").forEach(tick =>
        tick.textContent = duration > 0 ? formatTime(Number(tick.dataset.playerTick) * duration / 5) : "—");
    const seek = root.querySelector("[data-player-seek]");
    if (seek) {
        seek.disabled = duration <= 0;
        seek.value = duration > 0 ? elapsed * 100 / duration : 0;
        seek.setAttribute("aria-valuetext", `${formatTime(elapsed)} de ${formatTime(duration)}`);
    }
}
