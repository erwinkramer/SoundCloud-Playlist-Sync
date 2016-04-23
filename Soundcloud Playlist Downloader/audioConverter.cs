using System;
using System.Diagnostics;
using System.IO;
using NAudio.Lame;
using NAudio.MediaFoundation;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Soundcloud_Playlist_Downloader
{
    internal class AudioConverter
    {
        //NOTE  Default bitrate is set to 320 to keep the track high quality, 
        //      we won't use 128 because that would mean we can just 
        //      download the low quality stream (128 bit) and forget about converting
        private static readonly int bitRate = 320;
        private static readonly int sampleRate = 44100; //44100 Hz Sample rate 
        private static readonly int bitDepth = 16;
        private static readonly int channels = 2;
        private static int _uniqueTempFileCounter;

        public static bool ConvertAllTheThings(byte[] strangefile, ref Track song, string extension)
        {
            var directory = Path.GetDirectoryName(song.LocalPath);

            byte[] mp3bytes = null;

            if (extension == ".wav")
            {
                mp3bytes = ConvertWavToMp3(strangefile, directory);
                if (mp3bytes != null)
                {
                    song.LocalPath += ".mp3"; //conversion wil result in an mp3
                    File.WriteAllBytes(song.LocalPath, mp3bytes);
                    return true;
                }
                return false;
            }
            if (extension == ".aiff" || extension == ".aif")
            {
                var succesfullAiffConvert = false;
                succesfullAiffConvert = ConvertAiffToMp3(strangefile, directory, out mp3bytes);
                if (succesfullAiffConvert && mp3bytes != null)
                {
                    song.LocalPath += ".mp3"; //conversion wil result in an mp3
                    File.WriteAllBytes(song.LocalPath, mp3bytes);
                    return true;
                }
                return false;
            }
            if ((extension == ".m4a" || extension == ".aac") && isWindows8OrHigher())
            {
                return ConvertM4aToMp3(strangefile, directory, ref song);
            }
            return false;
        }

        public static byte[] ConvertWavToMp3(byte[] wavFile, string directory)
        {
            byte[] mp3bytes = null;
            var newFormat = new WaveFormat(bitRate, bitDepth, channels);

            try
            {
                _uniqueTempFileCounter += 1;
                var tempFile = Path.Combine(directory, "tempdata" + _uniqueTempFileCounter + ".wav");

                using (var ms = new MemoryStream(wavFile))
                using (var rdr = new WaveFileReader(ms))
                {
                    if (rdr.WaveFormat.BitsPerSample == 24)
                        //can't go from 24 bits wav to mp3 directly, create temporary 16 bit wav 
                    {
                        ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                        var resampler = new WdlResamplingSampleProvider(sampleprovider, sampleRate);
                            //sample to new sample rate
                        WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                        mp3bytes = ConvertWavToMp3(tempFile, true); //file to mp3 bytes
                    }
                    else
                    {
                        using (var retMs = new MemoryStream())
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

        public static byte[] ConvertWavToMp3(string wavFile, bool deleteWavAfter)
            //this method takes an actual wav file and converts it
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
                if (deleteWavAfter)
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

        public static bool ConvertAiffToMp3(byte[] aiffFile, string directory, out byte[] mp3bytes)
        {
            mp3bytes = null;
            var newFormat = new WaveFormat(bitRate, bitDepth, channels);
            try
            {
                _uniqueTempFileCounter += 1;
                var tempFile = Path.Combine(directory, "tempdata" + _uniqueTempFileCounter + ".wav");

                using (var ms = new MemoryStream(aiffFile))
                using (var rdr = new AiffFileReader(ms))
                {
                    if (rdr.WaveFormat.BitsPerSample == 24)
                        //can't go from 24 bits aif to mp3 directly, create temporary 16 bit wav 
                    {
                        ISampleProvider sampleprovider = new Pcm24BitToSampleProvider(rdr); //24 bit to sample
                        var resampler = new WdlResamplingSampleProvider(sampleprovider, sampleRate);
                            //sample to new sample rate
                        WaveFileWriter.CreateWaveFile16(tempFile, resampler); //sample to actual wave file
                        mp3bytes = ConvertWavToMp3(tempFile, true); //file to mp3 bytes                       
                    }
                    else
                    {
                        using (var retMs = new MemoryStream())
                        using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, bitRate))
                        {
                            rdr.CopyTo(wtr);
                            mp3bytes = retMs.ToArray();
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }

        public static bool ConvertM4aToMp3(byte[] m4aFile, string directory, ref Track song)
            //requires windows 8 or higher
        {
            var tempFile = Path.Combine(directory, "tempdata" + _uniqueTempFileCounter + ".m4a");
            //

            try
            {
                _uniqueTempFileCounter += 1;
                File.WriteAllBytes(tempFile, m4aFile);
                song.LocalPath += ".mp3"; //conversion wil result in an mp3
                using (var reader = new MediaFoundationReader(tempFile)) //this reader supports: MP3, AAC and WAV
                {
                    var AACtype = AudioSubtypes.MFAudioFormat_AAC;
                    var bitrates = MediaFoundationEncoder.GetEncodeBitrates(AACtype, reader.WaveFormat.SampleRate,
                        reader.WaveFormat.Channels);
                    MediaFoundationEncoder.EncodeToMp3(reader, song.LocalPath, bitrates[bitrates.GetUpperBound(0)]);
                }
                File.Delete(tempFile);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                return false;
            }
        }

        public static bool isWindows8OrHigher()
        {
            var win8version = new Version(6, 2, 9200, 0);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8version)
            {
                // its win8 or higher.
                return true;
            }
            return false;
        }
    }
}