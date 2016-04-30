using System;
using System.Collections.Generic;
using Soundcloud_Playlist_Downloader.JsonObjects;

namespace Soundcloud_Playlist_Downloader.Utils
{
    /// <Summary>
    /// Compares two classes based only on the properties of the parent.
    /// </Summary>
    public class CompareUtils : IEqualityComparer<SoundcloudBaseTrack>
    {
        #region IEqualityComparer<BaseClass> Members
        public bool Equals(SoundcloudBaseTrack a, SoundcloudBaseTrack b)
        {         
            if ((a.last_modified != b.last_modified) || 
                (a.commentable != b.commentable) ||
                (a.downloadable != b.downloadable) ||
                (a.id != b.id) ||
                (a.Artist != b.Artist) ||
                (a.Title != b.Title) ||
                (a.TrackCreatedWith != b.TrackCreatedWith) ||
                (a.Username != b.Username) ||
                (a.artwork_url != b.artwork_url) ||
                (a.attachments_uri != b.attachments_uri) ||
                (a.available_country_codes != b.available_country_codes) ||
                (a.bpm != b.bpm) ||
                (a.created_at != b.created_at) ||
                (a.description != b.description) ||
                (a.duration != b.duration) ||
                (a.genre != b.genre) ||
                (a.isrc != b.isrc) ||
                (a.license != b.license) ||
                (a.label != b.label) ||
                (a.tag_list != b.tag_list) ||
                (a.video_url != b.video_url) ||
                (a.kind != b.kind))
                return false;
            return true;
        }

        public int GetHashCode(SoundcloudBaseTrack obj)
        {
            return Tuple.Create(obj.id, obj.key_signature, obj.last_modified, obj.original_content_size).GetHashCode();         
        }
        #endregion
    }
}
