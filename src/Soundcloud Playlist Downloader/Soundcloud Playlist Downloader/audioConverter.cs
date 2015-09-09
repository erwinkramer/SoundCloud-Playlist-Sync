using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
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
            var newFormat = new WaveFormat(44100, 16, 1); //44100 Hz Sample rate & 16 Bits per sample
            try
            {
                //NOTE  Default bitrate is set to 320 to keep the track high quality, 
                //      we won't use 128 because that would mean we can just 
                //      download the low quality stream (128 bit) and forget about converting
                int bitRate = 320;

                using (var retMs = new MemoryStream())
                using (var ms = new MemoryStream(aiffFile))
                using (var rdr = new AiffFileReader(ms))
                //using (var pcmStream = WaveFormatConversionStream.CreatePcmStream(rdr)) //create stream to convert
                //using (var convertedStream = new WaveFormatConversionStream(new WaveFormat(44100, 16, pcmStream.WaveFormat.Channels), pcmStream))
                using (var wtr = new LameMP3FileWriter(retMs, newFormat, bitRate))
                {
                    if (rdr.WaveFormat.BitsPerSample == 24)
                    {
                        ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr);
                        SampleToWaveProvider16 pcm16Bit = new SampleToWaveProvider16(sampleprovider);
                        var rdrConverted = new WaveFileReader(pcm16Bit.ToString());
                        rdrConverted.CopyTo(wtr);
                    }
                    else
                    {
                        rdr.CopyTo(wtr);
                    }
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

    