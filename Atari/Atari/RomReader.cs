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
            var bytes = File.ReadAllBytes(@"C:\Users\David\Documents\Visual Studio 2015\Projects\Atari\Atari\SpaceInvaders.bin");
            var bytesReadable = BitConverter.ToString(bytes).Replace("-", " ");
            Console.Write(bytesReadable);
            Console.ReadLine();
            return bytes;
        }
    }
}
