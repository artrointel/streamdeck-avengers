/// body on loaded with settings data ///
function onLoad() {
	if (cfg == null || cfg.length == 0) {
		return;
	}
	// refer to FunctionConfigs.cs
	for (var idx = 1; idx <= cfg.length; idx++) {
        onAddNewFunction();
		var functionConfig = cfg[idx - 1];
		setSelectValue('sFunctionTrigger', idx, functionConfig['mTrigger']);
        setSelectValue('sFunctionType', idx, functionConfig['mType']);
        onFunctionChanged(idx);

        setValue('iFunctionDelay', idx, functionConfig['mDelay']);
        setValue('iFunctionDuration', idx, functionConfig['mDuration']);
        setValue('iFunctionInterval', idx, functionConfig['mInterval']);
		setValue('iFunctionMetadata', idx, functionConfig['mMetadata']);

		// for each options
		switch (functionConfig['mType']) {
			case 'Keycode':
				var keyCombination = "";
				var keycodes = functionConfig['mMetadata'].split(' '); // metadata contains keycodes with spaces. 
				for (var ki = 0; ki < keycodes.length; ki++) { // last index will be empty.
					keyCombination += gKeyboardMap[Number(keycodes[ki])] + '+';
				}
				if (keyCombination.length > 0) {
					keyCombination = keyCombination.slice(0, -1); // removes last char '+'
                }
				
				document.getElementById('iFunctionMetadata' + idx).innerHTML = keyCombination;
				break;
        }
	}
}

var idx = 1;

function onAddNewFunction() {
	var newFunctionItem = document.createElement('div');
	newFunctionItem.innerHTML =
		`<div class="sdpi-item" id="dFunctionContainer${idx}" name="functionItem">
			<select class="sdpi-item-value" id="sFunctionTrigger${idx}" style="width:50px">
				<option value="OnKeyPressed">OnKeyPressed</option>
			</select>
			<select class="sdpi-item-value" id="sFunctionType${idx}" onchange="onFunctionChanged(${idx})" style="width:50px">
				<option value="Select">Select</option>
				<option value="OpenFile">Open file/folder</option>
				<option value="OpenWebpage">Open Webpage</option>
				<option value="ExecuteCommand">Execute Command</option>
				<option value="Keycode">Key Combination</option>
				<option value="Text">Type Text</option>
			</select>

			<div class="sdpi-item-value avg-container-center">
				<button class="sdpi-item-value" id="iDelete${idx}" onclick="onBtnDelete(${idx})">Delete</button>
			</div>
		</div>`;

	var effectList = document.getElementById('dvFunctionList');
	effectList.appendChild(newFunctionItem.firstChild);

	idx++;
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
		// Creates option UI
		var optionDiv = null;
		var optionHr = document.createElement('hr');
		optionHr.id = `dOptionsHr${idx}`;
		if (type == 'OpenFile') {
			optionDiv = createOpenFileOptionsDiv(idx);
		}
		else if (type == 'OpenWebpage') {
			optionDiv = createOpenWebpageOptionsDiv(idx);
		}
		else if (type == 'ExecuteCommand') {
			optionDiv = createExecuteCommandOptionsDiv(idx);
		}
		else if (type == 'Keycode') {
			optionDiv = createKeyCombinationOptionsDiv(idx);
		}
		else if (type == 'Text') {
			optionDiv = createTextOptionsDiv(idx);
		}

		// attach the option UI
		if (optionDiv != null) {
			var container = document.getElementById('dFunctionContainer' + idx);
			container.parentNode.insertBefore(optionDiv, container.nextSibling);
			optionDiv.parentNode.insertBefore(optionHr, optionDiv.nextSibling);
        }
    }
}

function onBtnDelete(idx) {
    setSelectValue('sFunctionType', idx, 'Select');
    onFunctionChanged(idx);
    document.getElementById(`dFunctionContainer${idx}`).style.display = "none";
}

/// detail options ///

function createOpenFileOptionsDiv(idx) {
	var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	optionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Put path of a file or a folder to open`;

	var pathDiv = createSdpiChildDiv(groupDiv, 'path', idx, 'avg-container-center');
	pathDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iFunctionMetadata${idx}" type="text"
			placeholder="ex) C:\myProgram.exe or C:\MyDownloadFolder"/>`;

	return optionDiv;
}

function createOpenWebpageOptionsDiv(idx) {
	var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	optionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Put address of a webpage to open`;

	var addrDiv = createSdpiChildDiv(groupDiv, 'addr', idx, 'avg-container-center');
	addrDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iFunctionMetadata${idx}" type="text"
			placeholder="ex) google.com"/>`;

	return optionDiv;
}

function createExecuteCommandOptionsDiv(idx) {
	var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	optionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Put command to be executed`;

	var cmdDiv = createSdpiChildDiv(groupDiv, 'cmd', idx, 'avg-container-center');
	cmdDiv.innerHTML =
		`<textarea class="sdpi-item-value" id="iFunctionMetadata${idx}"
			placeholder="ex) shutdown -s -t 3600 or shutdown -a"></textarea>`;

	// TODO add check button whether in background or not
	return optionDiv;
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
				this.recordedKeycode += gKeyboardMap[keycode] + "+";
			}
		}
		if (this.recordedKeycodeASC.length > 0) {
			this.recordedKeycodeASC = this.recordedKeycodeASC.slice(0, -1); // removes last empty space
			this.recordedKeycode = this.recordedKeycode.slice(0, -1); // removes last char '+'
        }
    }
}
var keyRecorder;

function createKeyCombinationOptionsDiv(idx) {
	keyRecorder = new KeyRecorder;
	
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Record key combination and edit detail options.`;

	var keycodeDiv = createSdpiChildDiv(groupDiv, 'keycode', idx, 'avg-container-center');
	keycodeDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iKeyRecorder${idx}" type="text"
			placeholder="Record key here"
			onkeydown="keyRecorder.onKeyDown(${idx})" 
			onkeyup="keyRecorder.onKeyUp(${idx})"/>
		<label class="sdpi-item-value avg-label" id="iFunctionMetadata${idx}">(Type Any key)</label>`;
	
	var loopDiv = createSdpiChildDiv(groupDiv, 'loop', idx, 'avg-container-center');
	loopDiv.innerHTML =
        `<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Interval</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionInterval${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>`;
	return openOptionDiv;
}

function createTextOptionsDiv(idx) {
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Write text and edit detail options.`;

	var textDiv = createSdpiChildDiv(groupDiv, 'textString', idx, 'avg-container-center');
	textDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iFunctionMetadata${idx}" type="text"
			placeholder="write any text here."/>`;

	var loopDiv = createSdpiChildDiv(groupDiv, 'loop', idx, 'avg-container-center');
	loopDiv.innerHTML =
        `<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Interval</label>
		<input class="sdpi-item-value avg-input-text" id="iFunctionInterval${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>`;
	return openOptionDiv;
}

/// on apply and cancel button clicked ///

function onBtnCancelClicked() {
	window.close();
}

function onBtnApplyClicked() {
	// process saving all changes
	var payload = buildFunctionPayload();
	window.opener.sendPayloadToPlugin(payload);
	window.close();
}

function buildFunctionPayload() {
	var payload = {};
	var count = document.getElementsByName('functionItem').length;
	payload['payload_updateFunctions'] = 'true';
	payload['payload_arrayCount'] = count;
	if (count > 0) {
		for (var i = 1; i <= count; i++) {
			payload['sFunctionTrigger' + i] = getSelectValue('sFunctionTrigger', i);
			payload['sFunctionType' + i] = getSelectValue('sFunctionType', i);

			payload['iFunctionInterval' + i] = getValue('iFunctionInterval', i);
			payload['iFunctionDelay' + i] = getValue('iFunctionDelay', i);
			payload['iFunctionDuration' + i] = getValue('iFunctionDuration', i);
			payload['iFunctionMetadata' + i] = getValue('iFunctionMetadata', i);
		}
	}
	return payload;
}