using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public enum DeviceSwitchType
    {
        Camera_Switch,
        PLC_Switch,
        Scanner_Switch
    }

    public class DeviceSwitchStatusChangedEventArgs:EventArgs
    {
        
        public DeviceSwitchType SwitchType { get; }


        public bool IsOn { get; }
        public DeviceSwitchStatusChangedEventArgs(DeviceSwitchType switchType, bool isOn)
        {
            SwitchType = switchType;
            IsOn = isOn;
        }
    }
}
