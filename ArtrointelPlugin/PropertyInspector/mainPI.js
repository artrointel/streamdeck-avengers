function onSelectImage() {
    document.getElementById('inputFile').click();
}

function onImageUpdated(e) {
    var input = document.getElementById('inputFile');
    var label = document.getElementById('iSelectedImageFileName');
    label.value = input.files[0].name;

    var inputUri = decodeURIComponent(input.value.replace(/^C:\\fakepath\\/, ''));
    var payload = buildImageUpdatePayload(inputUri);
    sendPayloadToPlugin(payload);
}

function buildImageUpdatePayload(path) {
    var payload = {};
    payload['payload_updateImage'] = path;
    return payload;
}

function onBtnFunctionEditClicked() {
    var payload = window.open('functionPI/functionPI.html', 'Avengers Function Configuration');
}

function onBtnEffectEditClicked() {
    var payload = window.open('effectPI/effectPI.html', 'Avengers Function Configuration');
}