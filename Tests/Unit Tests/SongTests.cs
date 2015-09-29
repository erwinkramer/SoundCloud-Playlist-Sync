using NUnit.Framework;
using Soundcloud_Playlist_Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soundcloud_Playlist_Downloader.JsonPoco;

namespace Tests.Unit_Tests
{
    [TestFixture]
    class SongTests
    {

        private static Track song = null;

        [SetUp]
        public static void setup()
        {
            song = new Track();
        }


        [TestCase("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ~`!@#$%^&*()_+=-{}][\\|'\";:/?.>,,", 
            @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 and_-_")]
        public static void Song_Sanitize(string input, string expectedOutput)
        {
            //Assert.AreEqual(expectedOutput, song.Sanitize(input));
        }

    }
}
