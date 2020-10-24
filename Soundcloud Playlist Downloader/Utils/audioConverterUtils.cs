using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Lame;
using NAudio.MediaFoundation;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Soundcloud_Playlist_Downloader.JsonObjects;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class AudioConverterUtils
    {
        //NOTE  Default bitrate is set to 320 to keep the track high quality, 
        //      we won't use 128 because that would mean we can just 
        //      download the low quality stream (128 bit) and forget about converting
        private const int BitRate = 320;
        private const int SampleRate = 44100; //44100 Hz Sample rate 
        private static List<int> SupportedMPEGSampleRates = new List<int>() { 8000 , 11025 , 12000 , 16000 , 22050 , 24000 , 32000, 44100, 48000 };  //MPEG-1 + MPEG-2 + MPEG-2.5

        private static int _uniqueTempFileCounter;

        public static bool ConvertAllTheThings(byte[] strangefile, ref Track song, string extension)
        {
            var directory = Path.GetDirectoryName(song.LocalPath);
            byte[] mp3Bytes;

            if (extension == ".wav")
            {
                mp3Bytes = ConvertWavToMp3(strangefile);
                if (mp3Bytes != null)
                {
                    song.LocalPath += ".mp3"; //conversion resulted in an mp3
                    File.WriteAllBytes(song.LocalPath, mp3Bytes);
                    return true;
                }
                return false;
            }
            if (extension == ".aiff" || extension == ".aif")
            {
                var succesfullAiffConvert = ConvertAiffToMp3(strangefile, directory, out mp3Bytes);
                if (succesfullAiffConvert && mp3Bytes != null)
                {
                    song.LocalPath += ".mp3"; //conversion resulted in an mp3
                    File.WriteAllBytes(song.LocalPath, mp3Bytes);
                    return true;
                }
                return false;
            }
            if ((extension == ".m4a" || extension == ".aac") && IsWindows8_OrHigher())
            {
                return ConvertM4AToMp3(strangefile, directory, ref song);
            }
            return false;
        }

        public static byte[] ConvertWavToMp3(byte[] wavFile)
        {
            byte[] mp3Bytes = null;
            try
            {
                _uniqueTempFileCounter += 1;
                var tempFile = Path.GetTempFileName();

                using (var ms = new MemoryStream(wavFile))
                using (var rdr = new WaveFileReader(ms))
                {
                    if (rdr.WaveFormat.BitsPerSample == 24) //Can't go from 24 bits wav to mp3 directly, create temporary 16 bit wav 
                    {
                        ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                        var resampler = new WdlResamplingSampleProvider(sampleprovider, SampleRate); //sample to new sample rate
                        WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                        mp3Bytes = ConvertWavFileToMp3(tempFile, true); //file to mp3 bytes
                    }
                    else if (!SupportedMPEGSampleRates.Contains(rdr.WaveFormat.SampleRate)) //Can't go from unsupported Sample Rate wav to mp3 directly
                    {
                        var resampler = new WdlResamplingSampleProvider(rdr.ToSampleProvider(), SampleRate); //sample to new sample rate
                        WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                        mp3Bytes = ConvertWavFileToMp3(tempFile, true); //file to mp3 bytes
                    }
                    else
                    {
                        using (var retMs = new MemoryStream())
                        using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, BitRate))
                        {
                            rdr.CopyTo(wtr);
                            mp3Bytes = retMs.ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return mp3Bytes;
        }

        /// <summary>
        /// Takes an actual wav file and converts it
        /// </summary>
        /// <param name="wavFile"></param>
        /// <param name="deleteWavAfter"></param>
        /// <returns></returns>
        public static byte[] ConvertWavFileToMp3(string wavFile, bool deleteWavAfter)
        {
            byte[] mp3Bytes = null;
            try
            {
                using (var retMs = new MemoryStream())
                using (var rdr = new WaveFileReader(wavFile))
                using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, BitRate))
                {
                    rdr.CopyTo(wtr);
                    mp3Bytes = retMs.ToArray();
                }
                if (deleteWavAfter)
                {
                    File.Delete(wavFile);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return mp3Bytes;
        }

        public static bool ConvertAiffToMp3(byte[] aiffFile, string directory, out byte[] mp3Bytes)
        {
            mp3Bytes = null;
            try
            {
                _uniqueTempFileCounter += 1;
                var tempFile = Path.GetTempFileName();

                using (var ms = new MemoryStream(aiffFile))
                using (var rdr = new AiffFileReader(ms))
                {
                    //can't go from 24 bits aif to mp3 directly, create temporary 16 bit wav 
                    if (rdr.WaveFormat.BitsPerSample == 24)
                    {
                        ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                        var resampler = new WdlResamplingSampleProvider(sampleprovider, SampleRate); //sample to new sample rate
                        WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                        mp3Bytes = ConvertWavFileToMp3(tempFile, true); //file to mp3 bytes                       
                    }
                    else
                    {
                        using (var retMs = new MemoryStream())
                        using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, BitRate))
                        {
                            rdr.CopyTo(wtr);
                            mp3Bytes = retMs.ToArray();
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool ConvertM4AToMp3(byte[] m4AFile, string directory, ref Track song)
            //requires windows 8 or higher
        {
            var tempFile = Path.Combine(directory, "tempdata" + _uniqueTempFileCounter + ".m4a");
            //

            try
            {
                _uniqueTempFileCounter += 1;
                File.WriteAllBytes(tempFile, m4AFile);
                song.LocalPath += ".mp3"; //conversion wil result in an mp3
                using (var reader = new MediaFoundationReader(tempFile)) //this reader supports: MP3, AAC and WAV
                {
                    var aaCtype = AudioSubtypes.MFAudioFormat_AAC;
                    var bitrates = MediaFoundationEncoder.GetEncodeBitrates(aaCtype, reader.WaveFormat.SampleRate,
                        reader.WaveFormat.Channels);
                    MediaFoundationEncoder.EncodeToMp3(reader, song.LocalPath, bitrates[bitrates.GetUpperBound(0)]);
                }
                File.Delete(tempFile);
                return true;
            }
            catch (Exception)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                return false;
            }
        }

        private static bool IsWindows8_OrHigher()
        {
            var win8Version = new Version(6, 2, 9200, 0);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8Version)
            {
                return true; // its win8 or higher.
            }
            return false;
        }
    }
}