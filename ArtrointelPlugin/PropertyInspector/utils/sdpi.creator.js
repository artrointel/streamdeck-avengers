
function createSdpiDiv(id, idx, classes = null) {
	var div = document.createElement('div');
	div.className = 'sdpi-item';
	if (classes != null) {
		div.className += ' ' + classes;
    }
	div.id = `${id}${idx}`;
	return div;
}

function createSdpiGroupDiv(id, idx, preClasses = null, postClasses = null) {
	var div = document.createElement('div');
	div.className = 'sdpi-item-group';
	if (preClasses != null) {
		div.className = preClasses + ' ' + div.className;
    }
	if (postClasses != null) {
		div.className += ' ' + postClasses;
	}
	div.id = `${id}${idx}`;
	return div;
}

function createSdpiChildDiv(parentGroup, id, idx, classes = null) {
	var div = document.createElement('div');
	div.className = 'sdpi-item-child';
	if (classes != null) {
		div.className += ' ' + classes;
	}
	div.id = `${id}${idx}`;
	parentGroup.appendChild(div);
	return div;
}