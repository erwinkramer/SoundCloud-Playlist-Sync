using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Lame;
using System;
using System.Diagnostics;
using System.IO;

namespace Soundcloud_Playlist_Downloader
{
    class audioConverter
    {
        //NOTE  Default bitrate is set to 320 to keep the track high quality, 
        //      we won't use 128 because that would mean we can just 
        //      download the low quality stream (128 bit) and forget about converting
        private static int bitRate = 320;
        private static int sampleRate = 44100; //44100 Hz Sample rate 
        private static int bitDepth = 16;
        private static int channels = 2;
        private static int uniqueTempFileCounter = 0;

        public static byte[] ConvertAllTheThings(byte[] strangefile, string fullPath, string extension)
        {
            string directory = Path.GetDirectoryName(fullPath);

            byte[] mp3bytes = null;

            if (extension == ".wav")
            {
                mp3bytes = ConvertWavToMp3(strangefile);
            }
            if (extension == ".aiff" || extension == ".aif")
            {
                mp3bytes = ConvertAiffToMp3(strangefile, directory);
            }
            return mp3bytes;
        }

        public static byte[] ConvertWavToMp3(byte[] wavFile)
        {
            byte[] mp3bytes = null;
            try
            {         
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

        public static byte[] ConvertWavToMp3(string wavFile, bool deleteWavAfter) //this method takes an actual wav file and converts it
        {
            byte[] mp3bytes = null;
            try
            {             
                using (var retMs = new MemoryStream())
                using (var rdr = new WaveFileReader(wavFile))
                using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate))
                {
                    rdr.CopyTo(wtr);
                    mp3bytes = retMs.ToArray();
                }
                if(deleteWavAfter)
                {
                    File.Delete(wavFile);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return mp3bytes;
        }

        public static byte[] ConvertAiffToMp3(byte[] aiffFile, string directory) 
        {
            byte[] mp3bytes = null;
            var newFormat = new WaveFormat(bitRate, bitDepth, channels);
            try
            {
                uniqueTempFileCounter += 1;
                var tempFile = Path.Combine(directory, "tempdata"+ uniqueTempFileCounter +".wav");

                using (var retMs = new MemoryStream())
                using (var ms = new MemoryStream(aiffFile))
                using (var rdr = new AiffFileReader(ms))
                {
                    if (rdr.WaveFormat.BitsPerSample == 24) //can't go from 24 bits aif to mp3 directly, create temporary 16 bit wav 
                    {
                        using (var wtr = new WaveFileWriter(retMs, newFormat))
                        {
                            ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                            var resampler = new WdlResamplingSampleProvider(sampleprovider, sampleRate); //sample to new sample rate
                            WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                            mp3bytes = ConvertWavToMp3(tempFile, true); //file to mp3 bytes
                        }
                    }
                    else
                    {
                        using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate))
                        {
                            rdr.CopyTo(wtr);
                            mp3bytes = retMs.ToArray();
                        }                     
                    }                   
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

    