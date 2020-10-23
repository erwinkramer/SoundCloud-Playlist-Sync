using Soundcloud_Playlist_Downloader.JsonObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Soundcloud_Playlist_Downloader.Utils
{
    public class ProgressUtils
    {
        public int SongsToDownload;
        public int SongsDownloaded;
        public int SongsProcessing;
        public bool Completed { get; set; }
        public bool Aborted { get; set; }
        public bool IsAborting { get; set; }
        public bool IsExiting { get; set; }
        public bool IsError { get; set; }
        public ConcurrentQueue<Exception> Exceptions { get; set; }
        public int CurrentAmountOfExceptions { get; set; }
        public static double MaximumExceptionThreshHoldPercentage { get; set; }

        private ConcurrentDictionary<string, string> TrackProgress = new ConcurrentDictionary<string, string>();

        public ProgressUtils()
        {
            Completed = false;
            Aborted = false;
            IsExiting = false;
            IsError = false;
            IsAborting = false;
            SongsDownloaded = 0;
            MaximumExceptionThreshHoldPercentage = 15.00;
            SongsProcessing = 0;
            SongsToDownload = 0;
            TrackProgress = new ConcurrentDictionary<string, string>();
            Exceptions = new ConcurrentQueue<Exception>();
            CurrentAmountOfExceptions = 0;
        }

        public void AddOrUpdateFailedTrack(Track track, Exception e)
        {
            var exc = new Exception($"Exception while downloading track '{track.Title}' from artist '{track.Artist}'", e);
            this.CurrentAmountOfExceptions++;
            this.Exceptions.Enqueue(exc);
            this.TrackProgress.AddOrUpdate(track.id.ToString(), track.Title, (key, oldValue) => "[x] " + track.Title);
        }

        public void AddOrUpdateInProgressTrack(Track track)
        {
            this.TrackProgress.AddOrUpdate(track.id.ToString(), "[~] " + track.Title, (key, oldValue) => track.Title);
        }

        public void AddOrUpdateNotDownloadableTrack(Track track)
        {
            this.TrackProgress.AddOrUpdate(track.id.ToString(), track.Title, (key, oldValue) => "[o] " + track.Title);
        }

        public void AddOrUpdateSuccessFullTrack(Track track)
        {
            this.TrackProgress.AddOrUpdate(track.id.ToString(), track.Title, (key, oldValue) => "[✓] " + track.Title);
        }

        public ICollection<string> GetTrackProgressValues()
        {
            return this.TrackProgress.Values;
        }

        public void ThrowAllExceptionsWithMessage(string message)
        {
            ThrowAllExceptionsWithRootException(new Exception(message));
        }

        public void ThrowAllExceptionsWithRootException(Exception rootException)
        {
            if (CurrentAmountOfExceptions > 0)
                throw new AggregateException(rootException.Message, this.Exceptions);
            throw rootException;
        }

        public void ResetProgress()
        {
            SongsDownloaded = 0;
            SongsToDownload = 0;
            TrackProgress = new ConcurrentDictionary<string, string>();
            Exceptions = new ConcurrentQueue<Exception>();
            Aborted = false;
            Completed = false;
            IsAborting = false;
            IsError = false;
            CurrentAmountOfExceptions = 0;
        }
    }


}
