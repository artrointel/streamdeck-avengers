function onSelectImage() {
    document.getElementById('inputFile').click();
}

function onImageUpdated(e) {
    var input = document.getElementById('inputFile');
    var fReader = new FileReader();
    fReader.readAsDataURL(input.files[0]);
    fReader.onloadend = function (event) {

        var label = document.getElementById('iSelectedImageFileName');
        label.value = input.files[0].name;

        /* TODO make possible to send an image file from html.
        var preview = document.getElementById('testImage');
        preview.src = event.target.result;
        var payload = buildImageUpdatePayload(event.target.result);
        sendPayloadToPlugin(payload);
        */
    }
}

function buildImageUpdatePayload(base64img) {
    var payload = {};
    payload['payload_updateImage'] = base64img;
    alert(base64img);
    return payload;
}

function onBtnFunctionEditClicked() {
    var payload = window.open('functionPI/functionPI.html', 'Avengers Function Configuration');
}

function onBtnEffectEditClicked() {
    var payload = window.open('effectPI/effectPI.html', 'Avengers Function Configuration');
}