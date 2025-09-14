// Fensterbreite zurückgeben
window.getWindowWidth = () => window.innerWidth;

// Resize-Handler registrieren
window.registerResizeHandler = (dotnetHelper) => {
    function resizeListener() {
        dotnetHelper.invokeMethodAsync("OnResize");
    }
    window.addEventListener("resize", resizeListener);
    window._resizeListener = resizeListener;
};

// Resize-Handler deregistrieren
window.unregisterResizeHandler = () => {
    if (window._resizeListener) {
        window.removeEventListener("resize", window._resizeListener);
        delete window._resizeListener;
    }
};
