function onBtnCancelClicked() {
    if (confirm('This action will abort changes if you edited something in this page.')) {
        window.close();
    }
}

function onBtnApplyClicked() {
    // process saving all changes
}
var idx = 1;
function onAddNewEvent() {
    var eventList = document.getElementById('dvFunctionList');
	eventList.innerHTML +=
		`<div class="sdpi-item" id="dEvent${idx}">
			<select class="sdpi-item-value" id="sTrigger${idx}">
				<option value="onKeyPressed">OnKeyPressed</option>
				<option value="onKeyReleased">OnKeyReleased</option>
			</select>
			<select class="sdpi-item-value" id="sFunction${idx}">
				<option value="fOpenWebPage">Open Web Page</option>
				<option value="fExecuteProgram">Execute a program</option>
				<option value="fShortcutKey">Run shortcut key</option>
				<option value="fExecuteProgram">Play sound</option>
				<option value="fExecuteProgram">Execute batch command</option>
			</select>
			<button class="sdpi-item-value" id="btnConfiguration">Edit Config</button>
		</div>`;
	idx++;
}