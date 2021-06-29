/// body on loaded with settings data ///
function onLoad() {
	// refer to EffectConfigs.cs
	for (var idx = 1; idx <= cfg.length; idx++) {
		onAddNewEffect();
		var effectConfig = cfg[idx - 1];
		setSelectValue('sEffectTrigger', idx, effectConfig['mTrigger']);
		setSelectValue('sEffectType', idx, effectConfig['mType']);

		onEffectChanged(idx);
		setValue('iEffectRGB', idx, effectConfig['mHexRgb']);
		setValue('iEffectAlpha', idx, effectConfig['mAlpha']);
		setValue('iEffectDelay', idx, effectConfig['mDelay']);
		setValue('iEffectDuration', idx, effectConfig['mDuration']);
		setValue('iEffectMetadata', idx, effectConfig['mMetadata']);

		// for each options
		switch (effectConfig['mType']) {
			case 'Pie':
				var metadata = effectConfig['mMetadata'].split(' '); // metadata contains grow and clockwise option.
				document.getElementById('iGrow' + idx).checked = (metadata[0] === 'true');
				document.getElementById('iClockwise' + idx).checked = (metadata[1] === 'true');
				break;
		}
    }
}

var idx = 1;

function onAddNewEffect() {
	var newEffectItem = document.createElement('div');
	newEffectItem.innerHTML =
		`<div class="sdpi-item" id="dEffectContainer${idx}" name="effectItem">
			<select class="sdpi-item-value" id="sEffectTrigger${idx}" style="width:50px">
				<option value="OnKeyPressed">OnKeyPressed</option>
			</select>
			<select class="sdpi-item-value" id="sEffectType${idx}" onchange="onEffectChanged(${idx})" style="width:50px">
				<option value="Select">Select</option>
				<option value="Flash">Flash</option>
				<option value="ColorOverlay">Color overlay</option>
				<option value="CircleSpread">Circle Spread</option>
				<option value="Pie">Pie</option>
				<option value="BorderWave">Border Wave</option>
			</select>

			<div class="sdpi-item-value avg-container-center">
				<button class="sdpi-item-value" id="iDelete${idx}" onclick="onBtnDelete(${idx})">Delete</button>
			</div>
		</div>`;

	var effectList = document.getElementById('dvEffectList');
	effectList.appendChild(newEffectItem.firstChild);

	idx++;
}

function onEffectChanged(idx) {
	// remove prev option UI
	var prevOptions = document.getElementById('dOptions' + idx);
	var prevOptionsHr = document.getElementById('dOptionsHr' + idx);

	if (prevOptions != null) {
		prevOptions.remove();
		prevOptionsHr.remove();
	}

	var type = getSelectValue('sEffectType', idx);
	if (type != null) {
		// Creates option UI
		var optionDiv = null;
		var optionHr = document.createElement('hr');
		optionHr.id = `dOptionsHr${idx}`;
		if (type == 'Flash') {
			optionDiv = createFlashOptionsDiv(idx);
		}
		else if (type == 'ColorOverlay') {
			optionDiv = createColorOverlayOptionsDiv(idx);
		}
		else if (type == 'CircleSpread') {
			optionDiv = createCircleSpreadOptionsDiv(idx);
		}
		else if (type == 'Pie') {
			optionDiv = createPieOptionsDiv(idx);
		}
		else if (type == 'BorderWave') {
			optionDiv = createBorderWaveOptionsDiv(idx);
		}

		// attach the option UI
		if (optionDiv != null) {
			var container = document.getElementById('dEffectContainer' + idx);
			container.parentNode.insertBefore(optionDiv, container.nextSibling);
			optionDiv.parentNode.insertBefore(optionHr, optionDiv.nextSibling);
		}
	}
}

function onBtnDelete(idx) {
	setSelectValue('sEffectType', idx, 'Select');
	onEffectChanged(idx);
	document.getElementById(`dEffectContainer${idx}`).style.display = "none";
}

/// detail options ///
function _createRGBAPaletteDiv(groupDiv, idx) {
	var argbDiv = createSdpiChildDiv(groupDiv, 'rgba', idx, 'avg-container-center');
	argbDiv.innerHTML =
		`<label class="sdpi-item-value avg-label">Color</label>
		<input class="sdpi-item-value avg-input-color" id="iEffectRGB${idx}" type="color" value="#ffffff"/>
		<label class="sdpi-item-value avg-label">Transparency</label>
		<input class="sdpi-item-value avg-input-range-alpha" id="iEffectAlpha${idx}" type="range" min="0" max="255" value="200" />`;
	return argbDiv;
}

function _createAnimationPaletteDiv(groupDiv, idx) {
	var animDiv = createSdpiChildDiv(groupDiv, 'anim', idx, 'avg-container-center');
	animDiv.innerHTML =
		`<label class="sdpi-item-value avg-label">Delay</label>
		<input class="sdpi-item-value avg-input-text" id="iEffectDelay${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="0.0"/>
		<label class="sdpi-item-value avg-label">Duration</label>
		<input class="sdpi-item-value avg-input-text" id="iEffectDuration${idx}" type="number" min="0.0" step="0.001" placeholder="second" value="1.0"/>`;
	return animDiv;
}

function _createBasicOptionsDiv(idx) {
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);
	_createRGBAPaletteDiv(groupDiv, idx);
	_createAnimationPaletteDiv(groupDiv, idx);
	return openOptionDiv;
}

function createFlashOptionsDiv(idx) {
	return _createBasicOptionsDiv(idx);
}

function createColorOverlayOptionsDiv(idx) {
	return _createBasicOptionsDiv(idx);
}

function createCircleSpreadOptionsDiv(idx) {
	return _createBasicOptionsDiv(idx);
}

function createPieOptionsDiv(idx) {
	var openOptionDiv = createSdpiDiv('dOptions', idx, 'avg-container-center');
	var groupDiv = createSdpiGroupDiv('optionGroup', idx, 'sdpi-item-value');
	openOptionDiv.appendChild(groupDiv);
	_createRGBAPaletteDiv(groupDiv, idx);
	_createAnimationPaletteDiv(groupDiv, idx);

	var pieOptions = createSdpiChildDiv(groupDiv, 'pieOptions', idx, 'avg-container-center');
	pieOptions.innerHTML =
		`<input id="iEffectMetadata${idx}" type="text" value="false false" style="display:none"></div>
		<label class="sdpi-item-value avg-label">GrowPie</label>
		<input class="sdpi-item-value avg-checkbox" id="iGrow${idx}" type="checkbox" onchange="onChangePieMetadata(${idx})"/>
		<label class="sdpi-item-value avg-label">Clockwise</label>
		<input class="sdpi-item-value avg-checkbox" id="iClockwise${idx}" type="checkbox" onchange="onChangePieMetadata(${idx})"/>`;
	return openOptionDiv;
}

function onChangePieMetadata(idx) {
	var metadata = document.getElementById(`iEffectMetadata${idx}`);
	var grow = document.getElementById(`iGrow${idx}`);
	var clockwise = document.getElementById(`iClockwise${idx}`);
	metadata.value = grow.checked + ' ' + clockwise.checked;
}

function createBorderWaveOptionsDiv(idx) {
	return _createBasicOptionsDiv(idx);
}

/// on apply and cancel button clicked ///

function onBtnCancelClicked() {
	window.close();
}

function onBtnApplyClicked() {
	// process saving all changes
	var payload = buildEffectPayload();
	window.opener.sendPayloadToPlugin(payload);
	window.close();
}

function buildEffectPayload() {
	var payload = {};
	var count = document.getElementsByName('effectItem').length;
	payload['payload_updateEffects'] = 'true';
	payload['payload_arrayCount'] = count;
	if (count > 0) {
		for (var i = 1; i <= count; i++) {
			payload['sEffectTrigger' + i] = getSelectValue('sEffectTrigger', i);
			payload['sEffectType' + i] = getSelectValue('sEffectType', i);
			
			payload['iEffectRGB' + i] = getValue('iEffectRGB', i);
			payload['iEffectAlpha' + i] = getValue('iEffectAlpha', i);
			payload['iEffectDelay' + i] = getValue('iEffectDelay', i);
			payload['iEffectDuration' + i] = getValue('iEffectDuration', i);
			payload['iEffectMetadata' + i] = getValue('iEffectMetadata', i);
		}
	}
	return payload;
}