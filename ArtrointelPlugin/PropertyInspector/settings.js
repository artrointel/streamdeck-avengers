
class AvengersKeySettings {
    mBase64ImageString;
    mFunctionConfigurations;
    mEffectConfigurations;

    loadSettings(settings) {
        this.mBase64ImageString = settings['Base64ImageString'];
        this.mFunctionConfigurations = settings['FunctionConfigurations'];
        this.mEffectConfigurations = settings['EffectConfigurations'];

        var e = new Event('onSettingsUpdated');
        window.dispatchEvent(e);
    }
}