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
        private DirectSoundOut chan0 = null;
        private DirectSoundOut chan1 = null;
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
            output0 = new DirectSoundOut();
            output1 = new DirectSoundOut();

        }
    }
}
