/// body on loaded with settings data ///
function onLoad() {
	if (cfg == null || cfg.length == 0) {
		return;
	}
	// refer to ConditionConfigs.cs
	for (var idx = 1; idx <= cfg.length; idx++) {
        onAddNewCondition();
		var conditionConfig = cfg[idx - 1];
		setSelectValue('sCondition', idx, conditionConfig['mCondition']);
        setSelectValue('sBehavior', idx, conditionConfig['mBehavior']);
        onBehaviorChanged(idx);
	}
}

var idx = 1;

function onAddNewOption() {
    var newOptionItem = document.createElement('div');
	newOptionItem.innerHTML =
		`<div class="sdpi-item" id="dOptionContainer${idx}" name="conditionItem">
			<select class="sdpi-item-value" id="sCondition${idx}" style="width:50px">
				<option value="OnInterruption">OnKeyPressedWhileRunning</option>
                <option value="OnKeyLongPressed">OnKeyLongPressed</option>
			</select>
			<select class="sdpi-item-value" id="sBehavior${idx}" onchange="onBehaviorChanged(${idx})" style="width:50px">
				<option value="Select">Select</option>
				<option value="Restart">Restart</option>
				<option value="PauseResume">Pause/Resume</option>
				<option value="Stop">Stop</option>
			</select>

			<div class="sdpi-item-value avg-container-center">
				<button class="sdpi-item-value" id="iDelete${idx}" onclick="onBtnDelete(${idx})">Delete</button>
			</div>
		</div>`;

	var optionList = document.getElementById('dvOptionList');
    optionList.appendChild(newOptionItem.firstChild);

	idx++;
}

function onBehaviorChanged(idx) {
	// remove prev option UI
	var prevOptions = document.getElementById('dOptions' + idx);
	var prevOptionsHr = document.getElementById('dOptionsHr' + idx);
	
	if (prevOptions != null) {
		prevOptions.remove();
		prevOptionsHr.remove();
	}

    var type = getSelectValue('sBehavior', idx);
	if (type != null) {
		// Creates option UI
		var optionDiv = null;
		var optionHr = document.createElement('hr');
		optionHr.id = `dOptionsHr${idx}`;
		if (type == 'Restart') {
			optionDiv = createRestartOptionsDiv(idx);
		}
		else if (type == 'PauseResume') {
			optionDiv = createPauseResumeOptionsDiv(idx);
		}
		else if (type == 'Stop') {
			optionDiv = createStopOptionsDiv(idx);
		}
		// attach the option UI

		if (optionDiv != null) {
            var container = document.getElementById('dOptionContainer' + idx);
			container.parentNode.insertBefore(optionDiv, container.nextSibling);
			optionDiv.parentNode.insertBefore(optionHr, optionDiv.nextSibling);
        }
    }
}

function onBtnDelete(idx) {
    setSelectValue('sBehavior', idx, 'Select');
    onBehaviorChanged(idx);
    document.getElementById(`dOptionContainer${idx}`).style.display = "none";
}

/// detail options ///

function createRestartOptionsDiv(idx) {
	var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	optionDiv.appendChild(groupDiv);

	var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
	descDiv.innerHTML = `Restart this key`;

	return optionDiv;
}

function createPauseResumeOptionsDiv(idx) {
    var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
    var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
    optionDiv.appendChild(groupDiv);

    var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
    descDiv.innerHTML = `Pause/Resume this key if possible. This option can be used as toggle key.`;

    return optionDiv;
}

function createStopOptionsDiv(idx) {
    var optionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
    var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
    optionDiv.appendChild(groupDiv);

    var descDiv = createSdpiChildDiv(groupDiv, 'desc', idx, 'avg-center');
    descDiv.innerHTML = `Stop this key.`;

    return optionDiv;
}

/// on apply and cancel button clicked ///

function onBtnCancelClicked() {
	window.close();
}

function onBtnApplyClicked() {
	// process saving all changes
    var payload = buildOptionPayload();
	window.opener.sendPayloadToPlugin(payload);
	window.close();
}

function buildOptionPayload() {
	var payload = {};
	var count = document.getElementsByName('conditionItem').length;
    payload['payload_updateOptions'] = 'true';
	payload['meta_arrayCount'] = count;
	if (count > 0) {
		for (var i = 1; i <= count; i++) {
            payload['sCondition' + i] = getSelectValue('sCondition', i);
            payload['sBehavior' + i] = getSelectValue('sBehavior', i);
		}
	}
	return payload;
}