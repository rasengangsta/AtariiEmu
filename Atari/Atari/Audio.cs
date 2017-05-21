using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Atari
{
    class Audio
    {
        private DirectSoundOut chanOut0 = null;
        private DirectSoundOut chanOut1 = null;
        private BlockAlignReductionStream stream0 = null;
        private BlockAlignReductionStream stream1 = null;

        private void getAudio()
        {
            double vol0 = 0.1;//get from RAM
            double vol1 = 0.1;
            double frequency0 = 700;//get from RAM
            double frequency1 = 700;
            WaveTone tone0 = new WaveTone(frequency0, vol0);
            WaveTone tone1 = new WaveTone(frequency1, vol1);
            stream0 = new BlockAlignReductionStream(tone0);
            stream1 = new BlockAlignReductionStream(tone1);
            chanOut0 = new DirectSoundOut();
            chanOut1 = new DirectSoundOut();

            chanOut0.Init(stream0);
            chanOut0.Play();

            chanOut1.Init(stream1);
            chanOut1.Play();
        }

        public class WaveTone : WaveStream
        {
            private double frequency;
            private double amplitude;
            private double time;

            public WaveTone(double f, double a)
            {
                time = 0;
                frequency = f;
                amplitude = a;
            }

            public override long Position
            {
                get;
                set;
            }

            public override long Length
            {
                get
                {
                    return long.MaxValue;
                }
            }

            public override WaveFormat WaveFormat
            {
                get
                {
                    return new WaveFormat(44100, 16, 1);
                    //sample rate, number of bits
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                double shape = 1;
                int samples = count / 2;
                for (int i = 0; i < samples; i++)
                {
                    shape = amplitude * Math.Sin(Math.PI * 2 * frequency * time);
                        time = time + (1 / 44100);
                    short truncated = (short)Math.Round(shape * (Math.Pow(2, 15) - 1));
                    buffer[i * 2] = (byte)(truncated & 0x00ff);
                    buffer[i * 2 + 1] = (byte)((truncated & 0xff00) >> 8); 
                }
                return count;
            }
        }
    }
}
