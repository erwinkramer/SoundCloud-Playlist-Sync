using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader
{
    class audioConverter
    {

        public static byte[] ConvertAllTheThings(byte[] strangefile, string extension)
        {
            byte[] mp3bytes = null;

            if (extension == ".wav")
            {
                mp3bytes = ConvertWavToMp3(strangefile);
            }
            if (extension == ".aiff" || extension == ".aif")
            {
                mp3bytes = ConvertAiffToMp3(strangefile);
            }
            return mp3bytes;
        }

        public static byte[] ConvertWavToMp3(byte[] wavFile)
        {
            byte[] mp3bytes = null;
            try
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
                    mp3bytes = retMs.ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return mp3bytes;
        }
         
        public static byte[] ConvertAiffToMp3(byte[] aiffFile) //this method is not working for all cases
        {
            byte[] mp3bytes = null;
            try
            {
                //NOTE  Default bitrate is set to 320 to keep the track high quality, 
                //      we won't use 128 because that would mean we can just 
                //      download the low quality stream (128 bit) and forget about converting
                int bitRate = 320;
                using (var retMs = new MemoryStream())
                using (var ms = new MemoryStream(aiffFile))
                using (var rdr = new AiffFileReader(ms))
                using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate))
                {
                    rdr.CopyTo(wtr);
                    mp3bytes = retMs.ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return mp3bytes;      
        }
    }
}

    