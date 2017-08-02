using System;
using System.Collections.Generic;
using System.Drawing;
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

            RAM.loadMemory(bytes);

            byte[] byteInstructions = new byte[3];

            var startTime = DateTime.Now;
            CPU._regPC = 61440;
            while (true)
            {
             
                //catch
                if (CPU._regPC > 61850 && CPU._regPC <= 62043)
                {
                    var x = 1;
                }

                byteInstructions[0] = RAM.Memory[CPU._regPC];
                byteInstructions[1] = RAM.Memory[CPU._regPC+1];
                byteInstructions[2] = RAM.Memory[CPU._regPC+2];
                CPU.ProcessInstruction(byteInstructions);
            }
            Console.WriteLine(DateTime.Now - startTime);
            Console.WriteLine(CPU.clock);


        }

       

    }
}
