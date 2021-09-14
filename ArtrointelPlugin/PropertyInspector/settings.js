
class AvengersKeySettings {
    mBase64ImageString;
    mCommandConfigurations;
    mEffectConfigurations;

    loadSettings(settings) {
        this.mBase64ImageString = settings['Base64ImageString'];
        this.mCommandConfigurations = settings['CommandConfigurations'];
        this.mEffectConfigurations = settings['EffectConfigurations'];

        var e = new Event('onSettingsUpdated');
        window.dispatchEvent(e);
    }
}