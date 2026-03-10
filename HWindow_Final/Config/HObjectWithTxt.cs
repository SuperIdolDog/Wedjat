using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewROI.Config
{
    class HObjectWithTxt
    {
        private string dispTxt;
        private string color;
        private double row;//Row
        private double col;//Col


        public HObjectWithTxt(string _dispTxt, string _color, double _row, double _col)
        {
            dispTxt = _dispTxt;
            color = _color;
            row = _row;
            col = _col;
        }

        public string DispTxt
        {
            get { return dispTxt; }
            set { dispTxt = value; }
        }

        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public double Row
        {
            get { return row; }
            set { row = value; }
        }

        public double Col
        {
            get { return col; }
            set { col = value; }
        }

    }
}
