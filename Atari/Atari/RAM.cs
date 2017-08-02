using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atari
{
    public static class RAM
    {
        public static byte[] Memory = new byte[65535];

        public static void loadMemory(byte[] romInstructions)
        {
            bool switcher = false;
            byte[] memory = new byte[4096]; 
            while (memory.Length <= 65531)
            {
                byte[] emptybytes;
                if (switcher){
                    emptybytes = new byte[8192 - romInstructions.Length];
                }
                else
                {
                    emptybytes = new byte[0];
                    switcher = true;
                }
                    var s = new MemoryStream();
                s.Write(memory, 0, memory.Length);
                s.Write(emptybytes, 0, emptybytes.Length);
                s.Write(romInstructions, 0, romInstructions.Length);
                memory = s.ToArray();
            }
            Memory = memory;
           
        }
    }
}
