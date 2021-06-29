using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtrointelPlugin.Control;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArtrointelPlugin
{
    [PluginActionId("com.artrointel.avengerskey")]
    public class PluginAction : PluginBase
    {
        AvengersKeyController mController;
        
        public PluginAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            mController = new AvengersKeyController(payload.Settings, async (canvas) =>
            {
                await Connection.SetImageAsync(canvas.mImage);
            });

            Connection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
        }

        private void Connection_OnTitleParametersDidChange(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.TitleParametersDidChange> e)
        {

        }

        private void Connection_OnSendToPlugin(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            if(mController.handlePayload(e.Event.Payload))
            {
                SaveSettings();
            }
        }

        private void Connection_OnPropertyInspectorDidDisappear(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidDisappear> e)
        {

        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidAppear> e)
        {
            Connection.SendToPropertyInspectorAsync(JObject.FromObject(mController.getSettings()));
        }

        private void Connection_OnDeviceDidDisconnect(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidDisconnect> e)
        {

        }

        private void Connection_OnDeviceDidConnect(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidConnect> e)
        {

        }

        private void Connection_OnApplicationDidTerminate(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidTerminate> e)
        {

        }

        private void Connection_OnApplicationDidLaunch(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidLaunch> e)
        {

        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.DEBUG, "Plugin Dispose");
            Connection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
        }

        public async override void KeyPressed(KeyPayload payload)
        {
            mController.actionOnKeyPressed();
        }

        public async override void KeyReleased(KeyPayload payload) 
        {
            // mController.actionOnKeyReleased();
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.DEBUG, "ReceivedSettings," + payload.Settings);
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(mController.getSettings()));
        }

        #endregion
    }
}