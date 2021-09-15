
class AvengersKeySettings {
    mBase64ImageString;
    mCommandConfigurations;
    mEffectConfigurations;
    mOptionConfigurations;

    loadSettings(settings) {
        this.mBase64ImageString = settings['Base64ImageString'];
        this.mCommandConfigurations = settings['CommandConfigurations'];
        this.mEffectConfigurations = settings['EffectConfigurations'];
        this.mOptionConfigurations = settings['OptionConfigurations'];

        var e = new Event('onSettingsUpdated');
        window.dispatchEvent(e);
    }
}