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
            int romPointer = 0;
            var reader = new RomReader();
            var cpu = new CPU();

            var bytes = reader.Read();

            while (romPointer < bytes.Length - 2)
            {
                var nextInstruction = getNextInstruction(bytes, romPointer);
                romPointer = romPointer + nextInstruction.Length;
                cpu.ProcessInstruction(nextInstruction);
            }


        }

        private static byte[] getNextInstruction(byte[] bytes, int romPointer)
        {
            var hexByte = BitConverter.ToString(new[] { bytes[romPointer] }).Replace("-", " ");
            if (hexByte == "A8" || hexByte == "AA" || hexByte == "BA" || hexByte == "98" ||
                hexByte == "8A" || hexByte == "9A" || hexByte == "48" || hexByte == "08" ||
                hexByte == "68" || hexByte == "28" || hexByte == "CA" || hexByte == "88" ||
                hexByte == "0A" || hexByte == "4A" || hexByte == "2A" || hexByte == "6A" ||
                hexByte == "40" || hexByte == "60" || hexByte == "00" || hexByte == "18" ||
                hexByte == "58" || hexByte == "D8" || hexByte == "B8" || hexByte == "38" ||
                hexByte == "78" || hexByte == "F8" || hexByte == "EA")
                return new byte[1] { bytes[romPointer] };

            if (hexByte == "A9" || hexByte == "A2" || hexByte == "A0" || hexByte == "A5" ||
                hexByte == "B5" || hexByte == "A1" || hexByte == "B1" || hexByte == "A6" ||
                hexByte == "B6" || hexByte == "A4" || hexByte == "B4" || hexByte == "85" ||
                hexByte == "95" || hexByte == "81" || hexByte == "91" || hexByte == "86" ||
                hexByte == "96" || hexByte == "84" || hexByte == "94" || hexByte == "69" ||
                hexByte == "65" || hexByte == "75" || hexByte == "61" || hexByte == "71" ||
                hexByte == "E9" || hexByte == "E5" || hexByte == "F5" || hexByte == "E1" ||
                hexByte == "F1" || hexByte == "29" || hexByte == "25" || hexByte == "35" ||
                hexByte == "21" || hexByte == "31" || hexByte == "49" || hexByte == "45" ||
                hexByte == "55" || hexByte == "41" || hexByte == "51" || hexByte == "09" ||
                hexByte == "05" || hexByte == "15" || hexByte == "01" || hexByte == "11" ||
                hexByte == "C9" || hexByte == "C5" || hexByte == "D5" || hexByte == "C1" ||
                hexByte == "D1" || hexByte == "E0" || hexByte == "E4" || hexByte == "C0" ||
                hexByte == "C4" || hexByte == "24" || hexByte == "E6" || hexByte == "F6" ||
                hexByte == "C6" || hexByte == "D6" || hexByte == "06" || hexByte == "16" ||
                hexByte == "46" || hexByte == "56" || hexByte == "26" || hexByte == "36" ||
                hexByte == "66" || hexByte == "76" || hexByte == "10" || hexByte == "30" ||
                hexByte == "50" || hexByte == "70" || hexByte == "90" || hexByte == "B0" ||
                hexByte == "D0" || hexByte == "F0")
                return new byte[2] { bytes[romPointer], bytes[romPointer+1] };

            return new byte[3] { bytes[romPointer], bytes[romPointer + 1], bytes[romPointer + 2] };

        }

    }
}
