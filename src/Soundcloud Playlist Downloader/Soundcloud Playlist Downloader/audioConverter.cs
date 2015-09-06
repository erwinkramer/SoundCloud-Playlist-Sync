using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader
{
    class audioConverter
    {
        public static byte[] ConvertWavToMp3(byte[] wavFile)
        {
            //NOTE  Default bitrate is set to 320 to keep the track high quality, 
            //      we won't use 128 because that would mean we can just 
            //      download the low quality stream (128 bit) and forget about converting
            int bitRate = 320;
            using (var retMs = new MemoryStream())
            using (var ms = new MemoryStream(wavFile))
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate))
            {
                rdr.CopyTo(wtr);
                return retMs.ToArray();
            }
        }
    }
}
