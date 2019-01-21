//Language Manager class ⓒ Author by HongSic
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundcloud_Playlist_Downloader.Language
{
    public class LanguageManager
    {
        public static LanguageManager Language = GetDefault();

        Dictionary<string, string> lng = new Dictionary<string, string>();
        public LanguageManager() {}
        public LanguageManager(string[] Language)
        {
            for (int i = 0; i < Language.Length; i++)
            {
                if (!string.IsNullOrEmpty(Language[i]) &&
                    (Language[i][0] >= 'A' && Language[i][0] <= 'Z'))
                {
                    int index = Language[i].IndexOf('=');
                    if (index > 0)
                    {
                        string key = Language[i].Remove(index);
                        if (!Exist(key)) lng.Add(key, Language[i].Substring(index + 1).Replace("\\n", "\n"));
                    }
                }
            }
        }

        public string this[string Key] { get { return lng.ContainsKey(Key) ? lng[Key] : Key; }}
        public bool Exist(string Key) { return lng.ContainsKey(Key); }
        public int Count { get { return lng.Count; } }

        //Language: English (Defalut)
        //Author: HongSic(HSKernel)
        public static LanguageManager GetDefault()
        {
            LanguageManager m = new LanguageManager();
            //Main Area
            m.lng.Add("STR_ERR", "Error");
            m.lng.Add("STR_ABORT", "Abort");
            m.lng.Add("STR_SYNCHRONIZE", "Synchronize");

            m.lng.Add("STR_MAIN_TITLE_STABLE", "SoundCloud Playlist Sync {0} Stable");
            m.lng.Add("STR_MAIN_MENU_CONFIGS", "Configurations");
            m.lng.Add("STR_MAIN_MENU_CONFIG", "Config");
            m.lng.Add("STR_MAIN_MENU_CLIENT", "API Config");
            m.lng.Add("STR_MAIN_MENU_UPDATE", "Update");
            m.lng.Add("STR_MAIN_MENU_ABOUT", "About");
            m.lng.Add("STR_MAIN_MENU_LNG", "Language");

            m.lng.Add("STR_MAIN_BASIC", "Basic Option");
            m.lng.Add("STR_MAIN_BASIC_URL", "SoundCloud URL");
            m.lng.Add("STR_MAIN_BASIC_DIR", "Local Directory");
            m.lng.Add("STR_MAIN_BASIC_BROWSE", "Browse");
            m.lng.Add("STR_MAIN_BASIC_DM", "Download Method");
            m.lng.Add("STR_MAIN_BASIC_DM1", "All playlists from this user URL");
            m.lng.Add("STR_MAIN_BASIC_DM2", "All songs from this playlist URL");
            m.lng.Add("STR_MAIN_BASIC_DM3", "All songs favorited by the user at this profile URL");
            m.lng.Add("STR_MAIN_BASIC_DM4", "All songs by this artists URL");
            m.lng.Add("STR_MAIN_BASIC_DM5", "Single track URL (ignores sync method)");
            m.lng.Add("STR_MAIN_BASIC_SM", "Sync Method");
            m.lng.Add("STR_MAIN_BASIC_SM1", "One-way sync: Re-download locally removed songs");
            m.lng.Add("STR_MAIN_BASIC_SM2", "Two-way sync: Locally delete songs removed from SC");
            m.lng.Add("STR_MAIN_CONFSTAT", "Config State");
            m.lng.Add("STR_MAIN_CONF", "Configuration");
            m.lng.Add("STR_MAIN_CONFACTIVE", "Active");
            m.lng.Add("STR_MAIN_DOWMPROG", "Download Progress");
            
            m.lng.Add("STR_MAIN_ADVANCE", "Advance Option");
            m.lng.Add("STR_MAIN_ADVANCE_CONVERSE", "Conversion");
            m.lng.Add("STR_MAIN_ADVANCE_HQ", "Choose high quality versions if available");
            m.lng.Add("STR_MAIN_ADVANCE_HQ_MP3", "Convert high quality to MP3");
            m.lng.Add("STR_MAIN_ADVANCE_HQ_EXCL", "Exclude");
            m.lng.Add("STR_MAIN_ADVANCE_DOWNB", "Download Behaviour");
            m.lng.Add("STR_MAIN_ADVANCE_ILLIGCHAR", "Replace illegal characters in filename with equivalent instead of _");
            m.lng.Add("STR_MAIN_ADVANCE_ILLIGCHAR_DESC", "Characters to be replaced: / ? < > \\ : * | \nWill be replaced with Halfwidth and Fullwidth Forms");
            m.lng.Add("STR_MAIN_ADVANCE_CONCURRENCY", "Amount of concurrency");
            m.lng.Add("STR_MAIN_ADVANCE_OTHER", "Other");
            m.lng.Add("STR_MAIN_ADVANCE_FILEFORMAT", "Filename Formatter");
            m.lng.Add("STR_MAIN_ADVANCE_METAFORMAT", "Metadata Formatter (ID3)");
            m.lng.Add("STR_MAIN_ADVANCE_FBA", "Sort songs into folders by artist");
            m.lng.Add("STR_MAIN_ADVANCE_MSP", "Merge SoundCloud playlists");
            m.lng.Add("STR_MAIN_ADVANCE_GMPL", "Generate m3u8 playlist files");
            m.lng.Add("STR_MAIN_ADVANCE_MSTT", "Manifest save to Tag (ID3 [JSON])");

            m.lng.Add("STR_MAIN_STATUS_READY", "Ready");
            m.lng.Add("STR_MAIN_STATUS_COMPLETE", "Completed");
            m.lng.Add("STR_MAIN_STATUS_DOWNLOAD", "Synchronizing... {0} of {1} songs downloaded.");
            m.lng.Add("STR_MAIN_STATUS_SYNCED", "Tracks are already synchronized");
            m.lng.Add("STR_MAIN_STATUS_SYNCEDERROR", "An error prevented synchronization from starting");
            m.lng.Add("STR_MAIN_STATUS_ABORTING", "Aborting downloads... Please Wait.");
            m.lng.Add("STR_MAIN_STATUS_ABORTED", "Aborted");
            m.lng.Add("STR_MAIN_STATUS_FETCH_S", "s");
            m.lng.Add("STR_MAIN_STATUS_FETCH", "Fetching track{0} to download...");
            m.lng.Add("STR_MAIN_STATUS_CHECK", "Checking for track changes...");
            m.lng.Add("STR_MAIN_STATUS_INVALIDURL", "Invalid URL");
            m.lng.Add("STR_MAIN_STATUS_DIFFMANY", "Change settings or directory.");
            m.lng.Add("STR_MAIN_STATUS_NULLURL", "Enter the download url");
            m.lng.Add("STR_MAIN_STATUS_NULLDIR", "Enter local directory path");
            m.lng.Add("STR_MAIN_STATUS_EXIT", "Preparing for exit... Please Wait.");
            m.lng.Add("STR_MAIN_STATUS_SYNCING", "Sync is progressing... Are you sure to exit?");

            //Update status
            m.lng.Add("STR_UPDATE_AVAILABLE_TITLE", "Update Available");
            m.lng.Add("STR_UPDATE_AVAILABLE_TEXT", "An update is available. Would you like to update the application now?");
            m.lng.Add("STR_UPDATE_ERROR_TITLE", "Update Error");
            m.lng.Add("STR_UPDATE_ERROR_TEXT", "Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error");
            m.lng.Add("STR_UPDATE_NO_TITLE", "Update");
            m.lng.Add("STR_UPDATE_NO_TEXT", "No update available");
            m.lng.Add("STR_UPDATE_ERROR1_TITLE", "Update Checking Error");
            m.lng.Add("STR_UPDATE_ERROR1_TEXT", "Exception while checking for updates available. Exception thrown");

            //API Area
            m.lng.Add("STR_APICONFIG_TITLE", "API Config");
            m.lng.Add("STR_APICONFIG_DESC", "This setting changes your client ID.\nIf the API returns unauthorized it will be likely that the client ID is not working anymore.\nCreate a new ID by signing up for an application under the button.");
            m.lng.Add("STR_APICONFIG_ACTIVE", "Active");
            m.lng.Add("STR_APICONFIG_CLIENTID", "Client ID");
            m.lng.Add("STR_APICONFIG_CUSTOMID", "Custom ID");
            m.lng.Add("STR_APICONFIG_SAVE", "Save settings");
            m.lng.Add("STR_APICONFIG_LINK", "Click here to create new ID");

            //About Area
            m.lng.Add("STR_ABOUT_TITLE", "About Soundcloud Playlist Sync");
            m.lng.Add("STR_ABOUT_INFO", "A utility to synchronize local directories with SoundCloud playlists and favorited lists.");
            m.lng.Add("STR_ABOUT_COPYRIGHT", "Copyright © 2013-2019. The SoundCloud-Playlist-Sync project");
            m.lng.Add("STR_ABOUT_LICENSE", "GNU LESSER GENERAL PUBLIC LICENSE\n                       Version 3, 29 June 2007\n\n Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>\n Everyone is permitted to copy and distribute verbatim copies\n of this license document, but changing it is not allowed.\n\n\n  This version of the GNU Lesser General Public License incorporates\nthe terms and conditions of version 3 of the GNU General Public\nLicense, supplemented by the additional permissions listed below.\n\n  0. Additional Definitions.\n\n  As used herein, \"this License\" refers to version 3 of the GNU Lesser\nGeneral Public License, and the \"GNU GPL\" refers to version 3 of the GNU\nGeneral Public License.\n\n  \"The Library\" refers to a covered work governed by this License,\nother than an Application or a Combined Work as defined below.\n\n  An \"Application\" is any work that makes use of an interface provided\nby the Library, but which is not otherwise based on the Library.\nDefining a subclass of a class defined by the Library is deemed a mode\nof using an interface provided by the Library.\n\n  A \"Combined Work\" is a work produced by combining or linking an\nApplication with the Library.  The particular version of the Library\nwith which the Combined Work was made is also called the \"Linked\nVersion\".\n\n  The \"Minimal Corresponding Source\" for a Combined Work means the\nCorresponding Source for the Combined Work, excluding any source code\nfor portions of the Combined Work that, considered in isolation, are\nbased on the Application, and not on the Linked Version.\n\n  The \"Corresponding Application Code\" for a Combined Work means the\nobject code and/or source code for the Application, including any data\nand utility programs needed for reproducing the Combined Work from the\nApplication, but excluding the System Libraries of the Combined Work.\n\n  1. Exception to Section 3 of the GNU GPL.\n\n  You may convey a covered work under sections 3 and 4 of this License\nwithout being bound by section 3 of the GNU GPL.\n\n  2. Conveying Modified Versions.\n\n  If you modify a copy of the Library, and, in your modifications, a\nfacility refers to a function or data to be supplied by an Application\nthat uses the facility (other than as an argument passed when the\nfacility is invoked), then you may convey a copy of the modified\nversion:\n\n   a) under this License, provided that you make a good faith effort to\n   ensure that, in the event an Application does not supply the\n   function or data, the facility still operates, and performs\n   whatever part of its purpose remains meaningful, or\n\n   b) under the GNU GPL, with none of the additional permissions of\n   this License applicable to that copy.\n\n  3. Object Code Incorporating Material from Library Header Files.\n\n  The object code form of an Application may incorporate material from\na header file that is part of the Library.  You may convey such object\ncode under terms of your choice, provided that, if the incorporated\nmaterial is not limited to numerical parameters, data structure\nlayouts and accessors, or small macros, inline functions and templates\n(ten or fewer lines in length), you do both of the following:\n\n   a) Give prominent notice with each copy of the object code that the\n   Library is used in it and that the Library and its use are\n   covered by this License.\n\n   b) Accompany the object code with a copy of the GNU GPL and this license\n   document.\n\n  4. Combined Works.\n\n  You may convey a Combined Work under terms of your choice that,\ntaken together, effectively do not restrict modification of the\nportions of the Library contained in the Combined Work and reverse\nengineering for debugging such modifications, if you also do each of\nthe following:\n\n   a) Give prominent notice with each copy of the Combined Work that\n   the Library is used in it and that the Library and its use are\n   covered by this License.\n\n   b) Accompany the Combined Work with a copy of the GNU GPL and this license\n   document.\n\n   c) For a Combined Work that displays copyright notices during\n   execution, include the copyright notice for the Library among\n   these notices, as well as a reference directing the user to the\n   copies of the GNU GPL and this license document.\n\n   d) Do one of the following:\n\n       0) Convey the Minimal Corresponding Source under the terms of this\n       License, and the Corresponding Application Code in a form\n       suitable for, and under terms that permit, the user to\n       recombine or relink the Application with a modified version of\n       the Linked Version to produce a modified Combined Work, in the\n       manner specified by section 6 of the GNU GPL for conveying\n       Corresponding Source.\n\n       1) Use a suitable shared library mechanism for linking with the\n       Library.  A suitable mechanism is one that (a) uses at run time\n       a copy of the Library already present on the user's computer\n       system, and (b) will operate properly with a modified version\n       of the Library that is interface-compatible with the Linked\n       Version.\n\n   e) Provide Installation Information, but only if you would otherwise\n   be required to provide such information under section 6 of the\n   GNU GPL, and only to the extent that such information is\n   necessary to install and execute a modified version of the\n   Combined Work produced by recombining or relinking the\n   Application with a modified version of the Linked Version. (If\n   you use option 4d0, the Installation Information must accompany\n   the Minimal Corresponding Source and Corresponding Application\n   Code. If you use option 4d1, you must provide the Installation\n   Information in the manner specified by section 6 of the GNU GPL\n   for conveying Corresponding Source.)\n\n  5. Combined Libraries.\n\n  You may place library facilities that are a work based on the\nLibrary side by side in a single library together with other library\nfacilities that are not Applications and are not covered by this\nLicense, and convey such a combined library under terms of your\nchoice, if you do both of the following:\n\n   a) Accompany the combined library with a copy of the same work based\n   on the Library, uncombined with any other library facilities,\n   conveyed under the terms of this License.\n\n   b) Give prominent notice with the combined library that part of it\n   is a work based on the Library, and explaining where to find the\n   accompanying uncombined form of the same work.\n\n  6. Revised Versions of the GNU Lesser General Public License.\n\n  The Free Software Foundation may publish revised and/or new versions\nof the GNU Lesser General Public License from time to time. Such new\nversions will be similar in spirit to the present version, but may\ndiffer in detail to address new problems or concerns.\n\n  Each version is given a distinguishing version number. If the\nLibrary as you received it specifies that a certain numbered version\nof the GNU Lesser General Public License \"or any later version\"\napplies to it, you have the option of following the terms and\nconditions either of that published version or of any later version\npublished by the Free Software Foundation. If the Library as you\nreceived it does not specify a version number of the GNU Lesser\nGeneral Public License, you may choose any version of the GNU Lesser\nGeneral Public License ever published by the Free Software Foundation.\n\n  If the Library as you received it specifies that a proxy can decide\nwhether future versions of the GNU Lesser General Public License shall\napply, that proxy's public statement of acceptance of any version is\npermanent authorization for you to choose that version for the\nLibrary.");
            m.lng.Add("STR_ABOUT_OK", "OK");
            m.lng.Add("STR_ABOUT_TAB1", "License");
            m.lng.Add("STR_ABOUT_TAB2", "Project website(s)");
            m.lng.Add("STR_ABOUT_TAB3", "Translators");
            m.lng.Add("STR_ABOUT_TAB4", "Libraries");
            m.lng.Add("STR_ABOUT_PW_CM", "Current maintainer");
            m.lng.Add("STR_ABOUT_PW_OD", "Original developer");
            m.lng.Add("STR_ABOUT_PW_CON", "Contributors");
            m.lng.Add("STR_ABOUT_TRANSIMPL", "Implement function");

            //FileName Formatter
            m.lng.Add("STR_FORMAT_FILE_TITLE", "Implement");
            m.lng.Add("STR_FORMAT_FILE_SAMTR", "Sample tracks");
            m.lng.Add("STR_FORMAT_FILE_FOROP", "Format options");
            m.lng.Add("STR_FORMAT_FILE_FROMID3", "From ID3 Tag");
            m.lng.Add("STR_FORMAT_FILE_PREV", "Preview");
            m.lng.Add("STR_FORMAT_FILE_SAVE", "Save settings");
            m.lng.Add("STR_FORMAT_FILE_FTITLE", "Song Title");
            m.lng.Add("STR_FORMAT_FILE_FGENRE", "Song Genre");
            m.lng.Add("STR_FORMAT_FILE_FINDEX", "Song Index");
            m.lng.Add("STR_FORMAT_FILE_FUNAME", "User Name");
            m.lng.Add("STR_FORMAT_FILE_FCDATE", "Creation date");
            m.lng.Add("STR_FORMAT_FILE_FCTIME", "Creation time");
            m.lng.Add("STR_FORMAT_FILE_FEXT", "Song Extension (Format)");
            m.lng.Add("STR_FORMAT_FILE_FHD", "Song is HD quality");
            m.lng.Add("STR_FORMAT_FILE_FNAME", "Title (Name)");
            m.lng.Add("STR_FORMAT_FILE_FDESC", "Song Description");

            //Internal Area
            m.lng.Add("STR_DOWNLOAD_SONG_EX", "Number for concurrent downloads must be at least 1");
            m.lng.Add("STR_EXCEPTION_WEB1", "Could not get url {0}. \n\nServer returned http status code: {1}\n\nwith description {2}.");
            m.lng.Add("STR_EXCEPTION_WEB2", "Soundcloud API seems to be down, please check: http://status.soundcloud.com/ or https://developers.soundcloud.com/docs#errors for more information.\n\nThe following error was thrown: \n{0}");
            m.lng.Add("STR_EXCEPTION_WEB3", "The following error was thrown: {0}");
            m.lng.Add("STR_EXCEPTION_GET1", "Exception is null");
            m.lng.Add("STR_EXCEPTION_GET2", "Inner Exception");
            m.lng.Add("STR_EXCEPTION_GET3", "Aggregate Inner Exception");
            m.lng.Add("STR_EXCEPTION_GET4", "No inner exceptions found");
            m.lng.Add("STR_EXCEPTION_UPDATELOG", "Unable to update log");
            m.lng.Add("STR_EXCEPTION_JSONUTIL1", "Unable to find a matching playlist");
            m.lng.Add("STR_EXCEPTION_JSONUTIL2", "Errors occurred retrieving the playlist list information. Double check your url.");
            m.lng.Add("STR_EXCEPTION_JSONUTIL3", "Errors occurred retrieving the tracks list information. Double check your url.");
            m.lng.Add("STR_EXCEPTION_SYNC", "Unable to read manifest or to modify existing tracks. Occurred at track with EffectiveDownloadUrl: {0} and local path: {1}; exception: {2}");
            m.lng.Add("STR_PLISTUTIL_GENBY", "Generated by the SoundCloud Playlist Sync tool.");
            m.lng.Add("STR_PLISTUTIL_DEF", "Simple M3U8 playlist.");
            m.lng.Add("STR_PLISTUTIL_M3U_ML", "Most Liked (SC Downloader)");
            m.lng.Add("STR_PLISTUTIL_M3U_MP", "Most Played (SC Downloader)");
            m.lng.Add("STR_PLISTUTIL_M3U_RC", "Recently Changed (SC Downloader)");
            m.lng.Add("STR_PLISTUTIL_M3U_RD", "Recently Downloaded (SC Downloader)");
            m.lng.Add("STR_PLISTUTIL_M3U_OBS", "Ordered by SoundCloud (SC Downloader)");
            m.lng.Add("STR_PLISTUTIL_M3U_ERROR", "Unable to create Playlist (.m3u) file.");
            m.lng.Add("STR_PLISTUTIL_SORTML", "Sorted on most liked (on SoundCloud)");
            m.lng.Add("STR_PLISTUTIL_SORTMP", "Sorted on most played (on SoundCloud)");
            m.lng.Add("STR_PLISTUTIL_SORTSO", "Sorted SoundCloud order (on SoundCloud)");
            m.lng.Add("STR_PLISTUTIL_SORTRD", "Sorted on recently downloaded");
            m.lng.Add("STR_PLISTUTIL_SORTRC", "Sorted on recently changed");
            return m;
        }
    }
}
