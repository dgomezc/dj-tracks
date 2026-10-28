const cookieName = "djt-theme";

export function getTheme() {
    return document.documentElement.dataset.appTheme;
}

export function setTheme(theme) {
    document.documentElement.dataset.appTheme = theme;
    document.cookie = `${cookieName}=${encodeURIComponent(theme)}; path=/; max-age=31536000; samesite=lax`;
}
