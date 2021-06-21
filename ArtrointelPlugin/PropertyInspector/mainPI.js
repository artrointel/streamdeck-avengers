function onSelectImage() {
    document.getElementById('inputFile').click();
}

function onImageUpdated(e) {    
    var inputUri = decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, ''));
    const iFileName = inputUri.split('/').pop();
    
    var label = document.getElementById('iSelectedImageFileName');
    label.value = iFileName.length > 28
        ? iFileName.substr(0, 10)
        + '...'
        + iFileName.substr(iFileName.length - 10, iFileName.length)
        : iFileName;
    var payload = buildImageUpdatePayload(inputUri);
    sendPayloadToPlugin(payload);
}

function buildImageUpdatePayload(path) {
    var payload = {};
    payload['payload_updateImage'] = path;
    return payload;
}

function onBtnFunctionEditClicked() {
    var functionWindow = window.open('functionPI/functionPI.html', 'Avengers Function Configuration');
}

function onBtnEffectEditClicked() {
    var payload = window.open('effectPI/effectPI.html', 'Avengers Function Configuration');
}
