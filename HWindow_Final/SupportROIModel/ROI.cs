using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWindow.SupportROIModel
{
    [Serializable]
    public abstract class ROI
    {

        private string _Color = "";
        public string sColor
        {
            set { _Color = value; }
            get { return _Color; }
        }

        public abstract HRegion GenRegion();

        public abstract HXLDCont GenXld();

        public abstract HTuple GetTuple();

    }
}
