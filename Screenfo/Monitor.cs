using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screenfo
{
    public class Monitor
    {
        private string modelName;
        private string serialNumber;
        private int width;
        private int height;

        public Monitor(string modelName, string serialNumber, int width, int height)
        {
            this.modelName = modelName;
            this.serialNumber = serialNumber;
            this.width = width;
            this.height = height;
        }

        public string ModelName
        {
            get { return modelName; }
            set { modelName = value; }
        }

        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

    }
}
