using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Kee5Engine.Audio
{
    public class AudioManager
    {
        private static WaveOut music = null;
        private static List<WaveOut> sfxs = new List<WaveOut>();

        public static void PlayMusic(string file)
        {
            WaveFileReader reader = new WaveFileReader(file);
            LoopStream m = new LoopStream(reader);
            if (music != null)
            {
                music.Stop();
                music.Dispose();
            }
            music = new WaveOut();
            music.Init(m);
            music.Play();
        }

        public static void PlaySFX(string file)
        {
            WaveFileReader read = new WaveFileReader(file);
            WaveOut sfx = new WaveOut();
            sfx.Init(read);
            sfx.Play();
            sfxs.Add(sfx);
        }

        public static void Update()
        {
            if (sfxs.Count > 0)
            {
                List<WaveOut> nw = new List<WaveOut>();
                for (int i = sfxs.Count - 1; i >= 0; i--)
                {
                    if (sfxs[i].PlaybackState == PlaybackState.Stopped)
                    {
                        sfxs[i].Dispose();
                    }
                    else
                    {
                        nw.Add(sfxs[i]);
                    }
                }
                sfxs = nw;
            }
        }
    }

    internal class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat { get { return sourceStream.WaveFormat; } }
        public override long Length { get { return sourceStream.Length; } }
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while(totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        break;
                    }
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
