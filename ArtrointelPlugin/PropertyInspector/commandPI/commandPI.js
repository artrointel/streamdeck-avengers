/// body on loaded with settings data ///
function onLoad() {
	if (cfg == null || cfg.length == 0) {
		return;
	}
	// refer to CommandConfigs.cs
	for (var idx = 1; idx <= cfg.length; idx++) {
        onAddNewCommand();
		var commandConfig = cfg[idx - 1];
		setSelectValue('sCommandTrigger', idx, commandConfig['mTrigger']);
        setSelectValue('sCommandType', idx, commandConfig['mType']);
        onCommandChanged(idx);

        setValue('iCommandDelay', idx, commandConfig['mDelay']);
        setValue('iCommandDuration', idx, commandConfig['mDuration']);
        setValue('iCommandInterval', idx, commandConfig['mInterval']);
        setValue('iCommandMetadata', idx, commandConfig['mMetadata']);

		// for each options
        switch (commandConfig['mType']) {
			case 'Keycode':
				var keyCombination = "";
                var keycodes = commandConfig['mMetadata'].split(' '); // metadata contains keycodes with spaces. 
				for (var ki = 0; ki < keycodes.length; ki++) { // last index will be empty.
					keyCombination += gKeyboardMap[Number(keycodes[ki])] + '+';
				}
				if (keyCombination.length > 0) {
					keyCombination = keyCombination.slice(0, -1); // removes last char '+'
                }
				
				document.getElementById('iCommandMetadata' + idx).innerHTML = keyCombination;
				break;
			case 'VolumeControl':
                setSelectValue('sVolumeControl', idx, commandConfig['mMetadata']);
				break;
        }
	}
}

var idx = 1;

function onAddNewCommand() {
    var newCommandItem = document.createElement('div');
	newCommandItem.innerHTML =
		`<div class="sdpi-item" id="dCommandContainer${idx}" name="commandItem">
			<select class="sdpi-item-value" id="sCommandTrigger${idx}" style="width:50px">
				<option value="OnKeyPressed">OnKeyPressed</option>
			</select>
			<select class="sdpi-item-value" id="sCommandType${idx}" onchange="onCommandChanged(${idx})" style="width:50px">
				<option value="Select">Select</option>
				<option value="OpenFile">Open File/Folder</option>
				<option value="OpenWebpage">Open Webpage</option>
				<option value="ExecuteCommand">Execute Command</option>
				<option value="Keycode">Key Combination</option>
				<option value="Text">Type Text</option>
				<option value="PlaySound">Play Sound File</option>
				<option value="VolumeControl">Volume Control</option>
			</select>

			<div class="sdpi-item-value avg-container-center">
				<button class="sdpi-item-value" id="iDelete${idx}" onclick="onBtnDelete(${idx})">Delete</button>
			</div>
		</div>`;

	var effectList = document.getElementById('dvCommandList');
    effectList.appendChild(newCommandItem.firstChild);

	idx++;
}

function onCommandChanged(idx) {
	// remove prev option UI
	var prevOptions = document.getElementById('dOptions' + idx);
	var prevOptionsHr = document.getElementById('dOptionsHr' + idx);
	
	if (prevOptions != null) {
		prevOptions.remove();
		prevOptionsHr.remove();
	}

    var type = getSelectValue('sCommandType', idx);
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
		else if (type == 'PlaySound') {
			optionDiv = createPlaySoundDiv(idx);
        }
		else if (type == 'VolumeControl') {
			optionDiv = createVolumeControlDiv(idx);
		}
		// attach the option UI
		if (optionDiv != null) {
            var container = document.getElementById('dCommandContainer' + idx);
			container.parentNode.insertBefore(optionDiv, container.nextSibling);
			optionDiv.parentNode.insertBefore(optionHr, optionDiv.nextSibling);
        }
    }
}

function onBtnDelete(idx) {
    setSelectValue('sCommandType', idx, 'Select');
    onCommandChanged(idx);
    document.getElementById(`dCommandContainer${idx}`).style.display = "none";
}

/// detail options ///

function createOpenFileOptionsDiv(idx) {
	var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	optionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Put path of a file or a folder to open. click the button if you want to use file's icon in the stream deck.`;

	var pathDiv = createSdpiChildDiv(groupDiv, 'path', idx, 'avg-container-center');
	pathDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iCommandMetadata${idx}" type="text"
			placeholder="ex) C:\\myProgram.exe or C:\\MyDownloadFolder"/>`;

	var iconDiv = createSdpiChildDiv(groupDiv, 'icon', idx, 'avg-container-center');
	iconDiv.innerHTML =
		`<button type="button" value="Confirm" onclick="onUploadImageFromFileIcon(${idx})">Upload this file's icon as base image</button>`;
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
		`<input class="sdpi-item-value avg-input-text" id="iCommandMetadata${idx}" type="text"
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
		`<textarea class="sdpi-item-value" id="iCommandMetadata${idx}"
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
        var meta = document.getElementById('iCommandMetadata' + idx);
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
	descDiv.innerHTML = `Record key combination and edit detail options. 
		Note that some applications may block the macro key by security issue.`;

	var keycodeDiv = createSdpiChildDiv(groupDiv, 'keycode', idx, 'avg-container-center');
	keycodeDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iKeyRecorder${idx}" type="text"
			placeholder="Record key here"
			onkeydown="keyRecorder.onKeyDown(${idx})" 
			onkeyup="keyRecorder.onKeyUp(${idx})"/>
		<label class="sdpi-item-value avg-label" id="iCommandMetadata${idx}">(Type Any key)</label>`;
	
	var loopDiv = createSdpiChildDiv(groupDiv, 'loop', idx, 'avg-container-center');
	loopDiv.innerHTML =
        `<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Interval</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandInterval${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>`;
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
		`<input class="sdpi-item-value avg-input-text" id="iCommandMetadata${idx}" type="text"
			placeholder="write any text here."/>`;

	var loopDiv = createSdpiChildDiv(groupDiv, 'loop', idx, 'avg-container-center');
	loopDiv.innerHTML =
        `<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Interval</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandInterval${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>`;
	return openOptionDiv;
}

function createPlaySoundDiv(idx) {
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Play a sound file in background once.`;

	var textDiv = createSdpiChildDiv(groupDiv, 'textString', idx, 'avg-container-center');
	textDiv.innerHTML =
		`<input class="sdpi-item-value avg-input-text" id="iCommandMetadata${idx}" type="text"
			placeholder="ex) C:\\SoundFiles\\hello.mp3"/>`;

	var loopDiv = createSdpiChildDiv(groupDiv, 'loop', idx, 'avg-container-center');
	loopDiv.innerHTML =
		`<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Interval</label>
		<input class="sdpi-item-value avg-input-text" id="iCommandInterval${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>`;
	loopDiv.style.display = "none";
	return openOptionDiv;
}

function createVolumeControlDiv(idx) {
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Controls system volume.`;

	var textDiv = createSdpiChildDiv(groupDiv, 'textString', idx, 'avg-container-center');
	textDiv.innerHTML =
		`<input id="iCommandMetadata${idx}" type="text" style="display:none" value="VolumeUp" />
		<select class="sdpi-item-value" id="sVolumeControl${idx}" onchange="onVolumeControlChanged(${idx})" style="width:50px">
				<option value="VolumeUp">Volume Up</option>
				<option value="VolumeDown">Volume Down</option>
				<option value="ToggleMute">Toggle Mute</option>
		</select>`;

	return openOptionDiv;
}

function onVolumeControlChanged(idx) {
	var selected = getSelectValue('sVolumeControl', idx);
    document.getElementById(`iCommandMetadata${idx}`).value = selected;
}
/// on apply and cancel button clicked ///

function onBtnCancelClicked() {
	window.close();
}

function onBtnApplyClicked() {
	// process saving all changes
    var payload = buildCommandPayload();
	window.opener.sendPayloadToPlugin(payload);
	window.close();
}

function onUploadImageFromFileIcon(idx) {
    var metadata = document.getElementById('iCommandMetadata' + idx);
	var payload = {};
    payload['payload_updateImageFromFile'] = 'true';
	payload['meta_filePath'] = metadata.value;
	window.opener.sendPayloadToPlugin(payload);
}

function buildCommandPayload() {
	var payload = {};
	var count = document.getElementsByName('commandItem').length;
    payload['payload_updateCommands'] = 'true';
	payload['meta_arrayCount'] = count;
	if (count > 0) {
		for (var i = 1; i <= count; i++) {
            payload['sCommandTrigger' + i] = getSelectValue('sCommandTrigger', i);
            payload['sCommandType' + i] = getSelectValue('sCommandType', i);

            payload['iCommandInterval' + i] = getValue('iCommandInterval', i);
            payload['iCommandDelay' + i] = getValue('iCommandDelay', i);
			payload['iCommandDuration' + i] = getValue('iCommandDuration', i);
            payload['iCommandMetadata' + i] = getValue('iCommandMetadata', i);
		}
	}
	return payload;
}