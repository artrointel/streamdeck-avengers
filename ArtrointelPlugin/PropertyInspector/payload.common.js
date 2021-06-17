// payload helper

function getSelectValue(elemId, idx) {
    var selection = document.getElementById(elemId + idx);
    if (selection == null) {
        alert('dbg wrong selection:' + elemId + idx);
        return null;
    }
    return selection.options[selection.selectedIndex].value;
}

function getValue(elemId, idx, fallbackValue = 0.0) {
    var dataContainer = document.getElementById(elemId + idx);
    if (dataContainer == null) {
        return fallbackValue;
    }
    return dataContainer.value;
}