using System;
using System.IO;


namespace Atari
{
    class RomReader
    {
        public RomReader()
        {
            
        }

        public byte[] Read()
        {
            var bytes = File.ReadAllBytes(@"C:\Users\David\AtariiEmu\Atari\Atari\Spaceinvaders.bin");

            var bytesReadable = BitConverter.ToString(bytes).Replace("-", " ");
            Console.Write(bytesReadable);
            Console.ReadLine();

            return bytes;
        }
    }
}
