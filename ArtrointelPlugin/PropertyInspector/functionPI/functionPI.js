function onBtnCancelClicked() {
    if (confirm('This action will abort changes if you edited something in this page.')) {
        window.close();
    }
}

function onBtnApplyClicked() {
	// process saving all changes
	var payload = buildFunctionPayload();
	if (payload == null) {
		alert("Invalid data exists. cannot apply the effect.");
	} else {
		window.opener.sendPayloadToPlugin(payload);
		window.close();
    }
}

function buildFunctionPayload() {
	var payload = {};
	var count = document.getElementsByName('functionItem').length;
	
	if (count > 0) {
		payload['payload_updateFunctions'] = count;
		for (var i = 1; i <= count; i++) {
			payload['sFunctionTrigger' + i] = getSelectValue('sFunctionTrigger', i);
			payload['sFunctionType' + i] = getSelectValue('sFunctionType', i);

			payload['iFunctionInterval' + i] = getValue('iFunctionInterval', i);
			payload['iFunctionDelay' + i] = getValue('iFunctionDelay', i);
			payload['iFunctionDuration' + i] = getValue('iFunctionDuration', i);
			payload['iFunctionMetadata' + i] = getValue('iFunctionMetadata', i);
        }
		return payload;
	} else {
		return null;
    }
}

function onFunctionChanged(idx) {
	// remove prev option UI
	var prevOptions = document.getElementById('dOptions' + idx);
	var prevOptionsHr = document.getElementById('dOptionsHr' + idx);
	
	if (prevOptions != null) {
		prevOptions.remove();
		prevOptionsHr.remove();
	}

	var type = getSelectValue('sFunctionType', idx);
	if (type != null) {
		// create option UI
		var openOptionDiv = null;
		if (type == 'OpenFile') {
			openOptionDiv = createOpenFileOptionsDiv(idx);
		}
		else if (type == 'OpenWebpage') {
			openOptionDiv = createOpenWebpageOptionsDiv(idx);
		}
		else if (type == 'ExecuteCommand') {
			openOptionDiv = createExecuteCommandOptionsDiv(idx);
		}
		else if (type == 'Keycode') {
			openOptionDiv = createKeyCombinationOptionsDiv(idx);
		}
		else if (type == 'Text') {
			openOptionDiv = createTextOptionsDiv(idx);
		}
		else if (type == 'PlaySound') {
			openOptionDiv = createPlaySoundOptionsDiv(idx);
		}

		// attach the option UI
		if (openOptionDiv != null) {
			var container = document.getElementById('dFunctionContainer' + idx);
			container.parentNode.insertBefore(openOptionDiv, container.nextSibling);
        }
    }
}

function createOpenFileOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label style="width:100px; text-align:center;">Path to Open</label>
			<input class="sdpi-item-value" id="iFunctionMetadata${idx}" type="text"
				placeholder="type directory or file path to open. ex) C:\\test.exe"/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}

function createOpenWebpageOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label style="width:100px; text-align:center;">Path to Open</label>
			<input class="sdpi-item-value" id="iFunctionMetadata${idx}" type="text"
				placeholder="type address of webpage ex) google.com"/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}

function createExecuteCommandOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label style="width:100px; text-align:center;">Path to Open</label>
			<input class="sdpi-item-value" id="iFunctionMetadata${idx}" type="text"
				placeholder="runs bash command in background."/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}

class KeyRecorder {
	map = {};

	recordedKeycode = "";
	recordedKeycodeASC = "";

	onKeyDown(idx) {
		this.map[event.keyCode] = event.type == 'keydown';
		var meta = document.getElementById('iFunctionMetadata' + idx);
		this.recordResult();
		meta.value = this.recordedKeycodeASC;
		meta.innerHTML = this.recordedKeycode;
	}

	onKeyUp(idx) {
		this.map[event.keyCode] = event.type == 'keydown';
		var isDone = true;

		for (const keycode in this.map) {
			if (this.map[keycode] == true) {
				isDone = false;
			}
		}

		if (isDone) {
			document.getElementById('iKeyRecorder' + idx).value = "";
		}
	}

	recordResult() {
		this.recordedKeycode = "";
		this.recordedKeycodeASC = "";
		for (const keycode in this.map) {
			if (this.map[keycode] == true) {
				this.recordedKeycodeASC += keycode + " ";
				this.recordedKeycode += keyboardMap[keycode] + " ";
			}
		}
    }
}
var keyRecorder;

function createKeyCombinationOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	keyRecorder = new KeyRecorder;
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label id="iFunctionMetadata${idx}" style="width:150px; text-align:center;">Type Any key : </label>
			<input id="iKeyRecorder${idx}" type="text"
				placeholder="type key"
				onkeydown="keyRecorder.onKeyDown(${idx})" 
				onkeyup="keyRecorder.onKeyUp(${idx})"
				style="width:50px"/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}

function createTextOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label style="width:100px; text-align:center;">Text</label>
			<input class="sdpi-item-value" id="iFunctionMetadata${idx}" type="text"
				placeholder="write text here."/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}

function createPlaySoundOptionsDiv(idx) {
	var openOptionDiv = document.createElement('div');
	openOptionDiv.innerHTML =
		`<div class="sdpi-item" id="dOptions${idx}">
			<label style="width:100px; text-align:center;">Text</label>
			<input class="sdpi-item-value" id="iFunctionMetadata${idx}" type="text"
				placeholder="sound file path."/>
		 </div><hr id="dOptionsHr${idx}">`;
	return openOptionDiv;
}
var idx = 1;

function onAddNewEvent() {
	var newFunctionItem = document.createElement('div');
	newFunctionItem.innerHTML =
		`<div class="sdpi-item" id="dFunctionContainer${idx}" name="functionItem">
			<select class="sdpi-item-value" id="sFunctionTrigger${idx}" style="width:50px">
				<option>Select</option>
				<option value="OnKeyPressed">OnKeyPressed</option>
			</select>
			<select class="sdpi-item-value" id="sFunctionType${idx}" onchange="onFunctionChanged(${idx})" style="width:50px">
				<option>Select</option>
				<option value="OpenFile">Open file/folder</option>
				<option value="OpenWebpage">Open Webpage</option>
				<option value="ExecuteCommand">Execute Command</option>
				<option value="Keycode">Key Combination</option>
				<option value="Text">Type Text</option>
				<option value="PlaySound">Play sound file</option>
			</select>

			<div class="sdpi-item-value" style="text-align:center;">
				<input id="iFunctionDelay${idx}" type="text" placeholder="second" value="0.0" style="width:60px; height: 20px;"/>
			</div>
		</div>`;

	var effectList = document.getElementById('dvFunctionList');
	effectList.appendChild(newFunctionItem.firstChild);
		
	idx++;
}