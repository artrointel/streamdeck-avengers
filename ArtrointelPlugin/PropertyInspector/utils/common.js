function getSelectValue(elemId, idx) {
    var selection = document.getElementById(elemId + idx);
    return selection.options[selection.selectedIndex].value;
}

function setSelectValue(elemId, idx, value) {
    var selection = document.getElementById(elemId + idx);
    selection.value = value;
}

function getValue(elemId, idx, fallbackValue = 0.0) {
    var valueContainer = document.getElementById(elemId + idx);
    if (valueContainer == null) {
        return fallbackValue;
    }
    return valueContainer.value;
}

function setValue(elemId, idx, value) {
    var valueContainer = document.getElementById(elemId + idx);
    valueContainer.value = value;
}