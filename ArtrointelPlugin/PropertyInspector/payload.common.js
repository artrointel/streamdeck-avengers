// payload helper

function getSelectValue(elemId, idx) {
    var selection = document.getElementById(elemId + idx);
    if (selection == null) {
        alert('dbg wrong selection:' + elemId + idx);
        return null;
    }
    return selection.options[selection.selectedIndex].value;
}

function getDivValue(elemId, idx) {
    var dataContainer = document.getElementById(elemId + idx);
    if (dataContainer == null) {
        alert('dbg wrong data contained' + elemId + idx);
    }
    return dataContainer.value;
}