using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC_SYNC_Base.JsonObjects
{
    public class SyncSetting
    {
        private string soundcloudUrl;
        private string localDir;
        private SyncMethodType syncMethod;
        private DownloadMethodType downloadMethod;
        private bool highQualityVersions;
        private bool sortIntoFoldersByArtist;
        private bool includeArtistNameInFileName;
        private bool replaceIllegalCharactersWithEquivalent;
        private bool generatePlaylists;
        private int amountOfConcurrency;
        private bool convertHighQualityToMP3;
        private bool excludeM4a;
        private bool excludeAAC;

        public string SoundcloudUrl
        {
            get
            {
                return soundcloudUrl;
            }
            set
            {
                soundcloudUrl = value;
            }
        }

        public string LocalDir
        {
            get
            {
                return localDir;
            }
            set
            {
                localDir = value;
            }
        }
        public SyncMethodType SyncMethod
        {
            get
            {
                return syncMethod;
            }
            set
            {
                syncMethod = value;
            }
        }
        public DownloadMethodType DownloadMethod
        {
            get
            {
                return downloadMethod;
            }
            set
            {
                downloadMethod = value;
            }
        }
        public bool HighQualityVersions
        {
            get
            {
                return highQualityVersions;
            }
            set
            {
                highQualityVersions = value;
            }
        }
        public bool SortIntoFoldersByArtist
        {
            get
            {
                return sortIntoFoldersByArtist;
            }
            set
            {
                sortIntoFoldersByArtist = value;
            }
        }
        public bool IncludeArtistNameInFileName
        {
            get
            {
                return includeArtistNameInFileName;
            }
            set
            {
                includeArtistNameInFileName = value;
            }
        }

        public bool ReplaceIllegalCharactersWithEquivalent
        {
            get
            {
                return replaceIllegalCharactersWithEquivalent;
            }
            set
            {
                replaceIllegalCharactersWithEquivalent = value;
            }
        }
        public bool GeneratePlaylists
        {
            get
            {
                return generatePlaylists;
            }
            set
            {
                generatePlaylists = value;
            }
        }
        public int AmountOfConcurrency
        {
            get
            {
                return amountOfConcurrency;
            }
            set
            {
                amountOfConcurrency = value;
            }
        }
        public bool ConvertHighQualityToMP3
        {
            get
            {
                return convertHighQualityToMP3;
            }
            set
            {
                convertHighQualityToMP3 = value;
            }
        }
        public bool ExcludeM4a
        {
            get
            {
                return excludeM4a;
            }
            set
            {
                excludeM4a = value;
            }
        }
        public bool ExcludeAAC
        {
            get
            {
                return excludeAAC;
            }
            set
            {
                excludeAAC = value;
            }
        }


        public enum DownloadMethodType :int
        {
            AllSongsFromPlayList =0,
            AllSongsFavoritedByUserFromProfile = 1,
            AllSongsFromArtist = 2,
            SingleTrack = 3
        }
        public enum SyncMethodType :int
        {
            OneWaySync =0,
            TwoWaySync =1
        }


        public SyncSetting()
        {
            SoundcloudUrl = string.Empty;
            LocalDir = string.Empty;
            DownloadMethod = DownloadMethodType.SingleTrack;
            SyncMethod = SyncMethodType.OneWaySync;
            HighQualityVersions = true;
            SortIntoFoldersByArtist = true;
            IncludeArtistNameInFileName = true;
            ReplaceIllegalCharactersWithEquivalent = true;
            GeneratePlaylists = true;
            AmountOfConcurrency = 5;
            ConvertHighQualityToMP3 = true;
            ExcludeAAC = false;
            ExcludeM4a = false;
        }
    }
}
