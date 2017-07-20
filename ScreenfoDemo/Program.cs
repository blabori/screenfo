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
            try
            {
                List<Monitor> lst = Provider.GetConnectedScreens();
                Console.WriteLine("---------------");
                Console.WriteLine("| Screen info |");
                Console.WriteLine("---------------");

                for(int i = 0; i < lst.Count; i++)
                {
                    Console.WriteLine("Screen " + (i + 1) + ":");
                    Console.WriteLine("-- Serial number: {0}", lst[i].SerialNumber);
                    Console.WriteLine("-- Model name: {0}", lst[i].ModelName);
                    Console.WriteLine("-- Resolution: {0} x {1}", lst[i].Width, lst[i].Height);
                    Console.WriteLine();
                }
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("Error when querying connected screens: " + e.ToString());
            }
        }
    }
}
