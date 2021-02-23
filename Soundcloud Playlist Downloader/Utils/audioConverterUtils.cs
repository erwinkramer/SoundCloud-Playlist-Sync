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

        public static void ConvertHighQualityAudioFormats(MemoryStream soundMemoryStream, ref Track song, string extension)
        {
            var directory = Path.GetDirectoryName(song.LocalPath);

            if (extension == ".wav")
            {
                var convertedStream = ConvertWavToMp3(soundMemoryStream);
                song.LocalPath += ".mp3"; //conversion resulted in an mp3
                CopyToFile(song.LocalPath, convertedStream);
            }
            else if (extension == ".aiff" || extension == ".aif")
            {
                var convertedStream = ConvertAiffToMp3(soundMemoryStream, directory);
                song.LocalPath += ".mp3"; //conversion resulted in an mp3
                CopyToFile(song.LocalPath, convertedStream);
            }
            else if ((extension == ".m4a" || extension == ".aac") && IsWindows8_OrHigher())
            {
                ConvertM4AToMp3(soundMemoryStream, directory, ref song);
            }
        }

        private static void CopyToFile(string localPath, Stream soundStream)
        {
            if (soundStream == null || soundStream.Length == 0)
                throw new Exception("Sound stream was empty during check before writing");

            using (var fs = new FileStream(localPath, FileMode.Create))
            {
                soundStream.Position = 0;
                soundStream.CopyToAsync(fs).GetAwaiter().GetResult();
            }
        }

        public static Stream ConvertWavToMp3(Stream wavStream)
        {
            _uniqueTempFileCounter += 1;
            var tempFile = Path.GetTempFileName();

            using (var rdr = new WaveFileReader(wavStream))
            {
                if (rdr.WaveFormat.BitsPerSample == 24) //Can't go from 24 bits wav to mp3 directly, create temporary 16 bit wav 
                {
                    ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                    var resampler = new WdlResamplingSampleProvider(sampleprovider, SampleRate); //sample to new sample rate
                    WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                    return ConvertWavFileToMp3MemoryStream(tempFile, true); //file to mp3 bytes
                }
                else if (!SupportedMPEGSampleRates.Contains(rdr.WaveFormat.SampleRate)) //Can't go from unsupported Sample Rate wav to mp3 directly
                {
                    var resampler = new WdlResamplingSampleProvider(rdr.ToSampleProvider(), SampleRate); //sample to new sample rate
                    WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                    return ConvertWavFileToMp3MemoryStream(tempFile, true); //file to mp3 bytes
                }
                else
                {
                    var retMs = FilesystemUtils.recyclableMemoryStreamManager.GetStream();
                    using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, BitRate))
                    {
                        rdr.CopyTo(wtr);
                        return retMs;
                    }
                }
            }
        }

        /// <summary>
        /// Takes an actual wav file and converts it
        /// </summary>
        /// <param name="wavFile"></param>
        /// <param name="deleteWavAfter"></param>
        /// <returns></returns>
        public static Stream ConvertWavFileToMp3MemoryStream(string wavFile, bool deleteWavAfter)
        {
            var mp3MemoryStream = FilesystemUtils.recyclableMemoryStreamManager.GetStream();
            using (var rdr = new WaveFileReader(wavFile))
            using (var wtr = new LameMP3FileWriter(mp3MemoryStream, rdr.WaveFormat, BitRate))
            {
                rdr.CopyToAsync(wtr).GetAwaiter().GetResult();
            }
            if (deleteWavAfter)
            {
                File.Delete(wavFile);
            }
            return mp3MemoryStream;
        }

        public static Stream ConvertAiffToMp3(Stream aiffStream, string directory)
        {
            _uniqueTempFileCounter += 1;
            var tempFile = Path.GetTempFileName();
            using (var rdr = new AiffFileReader(aiffStream))
            {
                //can't go from 24 bits aif to mp3 directly, create temporary 16 bit wav 
                if (rdr.WaveFormat.BitsPerSample == 24)
                {
                    ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                    var resampler = new WdlResamplingSampleProvider(sampleprovider, SampleRate); //sample to new sample rate
                    WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                    return ConvertWavFileToMp3MemoryStream(tempFile, true); //file to mp3 bytes                       
                }
                else
                {
                    var retMs = FilesystemUtils.recyclableMemoryStreamManager.GetStream();
                    using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, BitRate))
                    {
                        rdr.CopyTo(wtr);
                        return retMs;
                    }
                }
            }
        }

        public static void ConvertM4AToMp3(Stream m4AStream, string directory, ref Track song)
            //requires windows 8 or higher
        {
            using (var reader = new StreamMediaFoundationReader(m4AStream)) //this reader supports: MP3, AAC and WAV
            {
                var aaCtype = AudioSubtypes.MFAudioFormat_AAC;
                var bitrates = MediaFoundationEncoder.GetEncodeBitrates(aaCtype, reader.WaveFormat.SampleRate,
                    reader.WaveFormat.Channels);

                song.LocalPath += ".mp3"; //conversion wil result in an mp3
                MediaFoundationEncoder.EncodeToMp3(reader, song.LocalPath, bitrates[bitrates.GetUpperBound(0)]);
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