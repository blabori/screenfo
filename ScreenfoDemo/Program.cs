using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screenfo;

namespace ScreenfoDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Monitor> lst = Provider.GetConnectedScreens();
            foreach (Monitor m in lst)
            {
                Console.WriteLine("---------------");
                Console.WriteLine("| Screen info |");
                Console.WriteLine("---------------");
                Console.WriteLine("Device name: {0}", m.InstanceName);
                Console.WriteLine("Serial number: {0}", m.SerialNumber);
                Console.WriteLine("Model name: {0}", m.ModelName);
                Console.WriteLine("Resolution: {0} x {1}\n", m.Width, m.Height);
            }
            Console.ReadKey();
        }
    }
}
