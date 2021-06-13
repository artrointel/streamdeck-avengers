function resetCounter() {
    var payload = {};
    payload.property_inspector = 'resetCounter';
    sendPayloadToPlugin(payload);
}

function onBtnFunctionEditClicked() {
    var result = window.open('functionPI/functionPI.html', 'Avengers Function Configuration');
}

function onBtnEffectEditClicked() {
    var result = window.open('effectPI/effectPI.html', 'Avengers Function Configuration');
}