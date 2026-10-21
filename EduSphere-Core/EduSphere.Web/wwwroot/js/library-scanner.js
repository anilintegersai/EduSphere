(() => {
    const video = document.getElementById('library-camera');
    const start = document.getElementById('start-scan');
    const stop = document.getElementById('stop-scan');
    const input = document.getElementById('scanner-value');
    const result = document.getElementById('scan-result');
    if (!video || !start || !stop || !input || !result) return;
    let stream;
    let active = false;
    const show = value => { input.value = value; result.textContent = `Detected: ${value}`; input.focus(); };
    const scan = async () => {
        if (!active || !('BarcodeDetector' in window)) return;
        try {
            const detector = new BarcodeDetector({ formats: ['qr_code', 'code_128', 'code_39', 'ean_13', 'ean_8', 'upc_a'] });
            const codes = await detector.detect(video);
            if (codes.length) { show(codes[0].rawValue); active = false; }
        } catch { result.textContent = 'Camera scan failed. Use the hardware scanner input.'; active = false; }
        if (active) requestAnimationFrame(scan);
    };
    start.addEventListener('click', async () => {
        if (!navigator.mediaDevices?.getUserMedia || !('BarcodeDetector' in window)) { result.textContent = 'Camera barcode detection is not supported by this browser. Hardware scanners remain available.'; input.focus(); return; }
        stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: { ideal: 'environment' } }, audio: false });
        video.srcObject = stream; await video.play(); active = true; requestAnimationFrame(scan);
    });
    stop.addEventListener('click', () => { active = false; stream?.getTracks().forEach(track => track.stop()); video.srcObject = null; });
    input.addEventListener('keydown', event => { if (event.key === 'Enter') { event.preventDefault(); show(input.value.trim()); } });
})();
