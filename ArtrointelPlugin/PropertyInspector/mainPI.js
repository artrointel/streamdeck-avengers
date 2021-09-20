// on avgSettings is updated
window.addEventListener('onSettingsUpdated', function (e) {
    document.getElementById('btnSelect').innerHTML = 'Update';
    document.getElementById('btnCommandEdit').disabled = false;
    document.getElementById('btnEffectEdit').disabled = false;
    document.getElementById('btnOptionEdit').disabled = false;

    var fCount = avgSettings.mCommandConfigurations.length;
    var eCount = avgSettings.mEffectConfigurations.length;
    var oCount = avgSettings.mOptionConfigurations.length;
    
    if (fCount > 0) {
        document.getElementById('btnCommandEdit').innerHTML = 'Edit  (' + fCount + ')';
    } else {
        document.getElementById('btnCommandEdit').innerHTML = 'Edit';
    }
    if (eCount > 0) {
        document.getElementById('btnEffectEdit').innerHTML = 'Edit  (' + eCount + ')';
    } else {
        document.getElementById('btnEffectEdit').innerHTML = 'Edit';
    }

    if (oCount > 0) {
        document.getElementById('btnOptionEdit').innerHTML = 'Edit  (' + oCount + ')';
    } else {
        document.getElementById('btnOptionEdit').innerHTML = 'Edit';
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
    payload['payload_updateImage'] = 'true';
    payload['meta_filePath'] = path;
    return payload;
}

// open an external window with loaded settings data if it exists

function onBtnCommandEditClicked() {
    var commandWindow = window.open('commandPI/commandPI.html', 'Avengers Command Configuration');
    commandWindow.cfg = avgSettings.mCommandConfigurations;
}

function onBtnEffectEditClicked() {
    var effectWindow = window.open('effectPI/effectPI.html', 'Avengers Effect Configuration');
    effectWindow.cfg = avgSettings.mEffectConfigurations;
}

function onBtnOptionEditClicked() {
    var effectWindow = window.open('optionPI/optionPI.html', 'Avengers Option Configuration');
    effectWindow.cfg = avgSettings.mOptionConfigurations;
}