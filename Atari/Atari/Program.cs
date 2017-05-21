using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atari
{
    class Program
    {

        

        static void Main(string[] args)
        {
            var reader = new RomReader();
            

            

            var bytes = reader.Read();
            byte[] byteInstructions = new byte[3];

            var startTime = DateTime.Now;

            while (CPU._regPC < bytes.Length - 1)
            {
                
                byteInstructions[0] = bytes[CPU._regPC];
                byteInstructions[1] = bytes[CPU._regPC+1];
                byteInstructions[2] = bytes[CPU._regPC+2];
                CPU.ProcessInstruction(byteInstructions);
            }
            Console.WriteLine(DateTime.Now - startTime);
            Console.WriteLine(CPU.clock);


        }

       

    }
}
