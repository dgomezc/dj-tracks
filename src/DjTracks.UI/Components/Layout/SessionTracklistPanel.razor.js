function currentAudio(version) {
    const audio = document.getElementById("djtracks-audio");
    return audio && audio.dataset.playbackVersion === String(version) ? audio : null;
}

export function getPosition(version) {
    const audio = currentAudio(version);
    return audio && audio.readyState > 0 && Number.isFinite(audio.currentTime) ? audio.currentTime : null;
}

export function seek(version, seconds) {
    const audio = currentAudio(version);
    if (!audio || !Number.isFinite(audio.duration) || seconds < 0 || seconds > audio.duration) return false;
    audio.currentTime = seconds;
    return true;
}
