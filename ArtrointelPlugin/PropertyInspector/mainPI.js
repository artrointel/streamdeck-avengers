// on avgSettings is updated
window.addEventListener('onSettingsUpdated', function (e) {
    document.getElementById('btnSelect').innerHTML = 'Update';
    document.getElementById('btnFunctionEdit').disabled = false;
    document.getElementById('btnEffectEdit').disabled = false;

    var fCount = avgSettings.mFunctionConfigurations.length;
    var eCount = avgSettings.mEffectConfigurations.length;
    
    if (fCount > 0) {
        document.getElementById('btnFunctionEdit').innerHTML = 'Edit [' + fCount + ']';
    }
    if (eCount > 0) {
        document.getElementById('btnEffectEdit').innerHTML = 'Edit [' + eCount + ']';
    }
});

// to upload image functions
function onSelectImage() {
    document.getElementById('inputFile').click();
}

function onImageUpdated(e) {
    var inputUri = decodeURIComponent(e.value.replace(/^C:\\fakepath\\/, ''));
    var payload = buildImageUpdatePayload(inputUri);
    sendPayloadToPlugin(payload);
}

function buildImageUpdatePayload(path) {
    var payload = {};
    payload['payload_updateImage'] = path;
    return payload;
}

// open external function window and effect window with loaded settings data if it exists
function onBtnFunctionEditClicked() {
    var functionWindow = window.open('functionPI/functionPI.html', 'Avengers Function Configuration');
    functionWindow.cfg = avgSettings.mFunctionConfigurations;
}

function onBtnEffectEditClicked() {
    var effectWindow = window.open('effectPI/effectPI.html', 'Avengers Function Configuration');
    effectWindow.cfg = avgSettings.mEffectConfigurations;
}
