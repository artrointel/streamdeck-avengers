function resetCounter() {
    var payload = {};
    payload.property_inspector = 'resetCounter';
    sendPayloadToPlugin(payload);
}

function onBtnCancelClicked() {
    if (confirm('This action will abort changes if you edited something in this page.')) {
        window.close();
    }
}

function onBtnApplyClicked() {
    // process saving all changes
}
var id = 1;
function onAddNewEvent() {
    var eventList = document.getElementById('dvEffectList');
	eventList.innerHTML +=
		`<div class="sdpi-item" id="dEvent${id}">
			<select class="sdpi-item-value" id="sTrigger${id}">
				<option value="onKeyPressed">OnKeyPressed</option>
				<option value="onKeyReleased">OnKeyReleased</option>
			</select>
			<select class="sdpi-item-value" id="sFunction${id}">
				<option value="fOpenWebPage">Image Update</option>
				<option value="fOpenWebPage">Flash</option>
				<option value="fExecuteProgram">Splash Circle</option>
				<option value="fShortcutKey">Pie</option>
				<option value="fExecuteProgram">Border Wave</option>
			</select>
			<button class="sdpi-item-value" id="btnConfiguration${id}">Edit Config</button>
		</div>`;
	id++;
}