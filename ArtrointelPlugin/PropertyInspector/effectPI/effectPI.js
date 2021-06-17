function onBtnCancelClicked() {
    if (confirm('This action will abort changes if you edited something in this page.')) {
        window.close();
    }
}

function onBtnApplyClicked() {
	// process saving all changes
	var payload = buildEffectPayload();
	if (payload == null) {
		alert("Invalid data exists. cannot apply the effect.");
	} else {
		window.opener.sendPayloadToPlugin(payload);
		window.close();
    }
}

function buildEffectPayload() {
	var payload = {};
	var count = document.getElementsByName('effectItem').length;
	
	if (count > 0) {
		payload['payload_updateEffects'] = count;
		for (var i = 1; i <= count; i++) {
			payload['sEffectTrigger' + i] = getSelectValue('sEffectTrigger', i);
			payload['sEffectType' + i] = getSelectValue('sEffectType', i);
			// TODO
			payload['iEffectRGB' + i] = getValue('iEffectRGB', i);
			payload['iEffectAlpha' + i] = getValue('iEffectAlpha', i);
			payload['iEffectDelay' + i] = getValue('iEffectDelay', i);
			payload['iEffectDuration' + i] = getValue('iEffectDuration', i);
        }
		return payload;
	} else {
		return null;
    }
}

var idx = 1;
function onAddNewEvent() {
	var newEffectItem = document.createElement('div');
	newEffectItem.innerHTML =
		`<div class="sdpi-item" id="dEvent${idx}" name="effectItem">
			<select class="sdpi-item-value" id="sEffectTrigger${idx}" style="width:50px">
				<option value="OnKeyPressed">OnKeyPressed</option>
				<option value="OnKeyReleased">OnKeyReleased</option>
			</select>
			<select class="sdpi-item-value" id="sEffectType${idx}" style="width:50px">
				<option value="ImageBlending">Image Blending</option>
				<option value="Flash">Flash</option>
				<option value="CircleSpread">Circle Spread</option>
				<option value="Pie">Pie</option>
				<option value="BorderWave">Border Wave</option>
			</select>

			<div class="sdpi-item-value" style="margin:0px;">
				<input id="iEffectRGB${idx}" type="color" />
				<input id="iEffectAlpha${idx}" type="range" min="0" max="255" value="200" style="width:50px; height:25px; margin-top:0px;"/>
			</div>
			<div class="sdpi-item-value" style="margin:0px;">
				<input id="iEffectDelay${idx}" type="text" placeholder="second" value="0.0" style="width:40px; height: 20px;"/>
				<input id="iEffectDuration${idx}" type="text" placeholder="second" value="0.5" style="width:40px; height: 20px;"/>
			</div>
		</div><hr />`;

	var effectList = document.getElementById('dvEffectList');
	effectList.appendChild(newEffectItem.firstChild);
		
	idx++;
}