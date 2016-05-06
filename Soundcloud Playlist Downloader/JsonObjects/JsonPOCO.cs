namespace Soundcloud_Playlist_Downloader.JsonObjects
{
    public class PaginatedCollectionPlaylist
    {
        public PlaylistItem[] Collection { get; set; }
        public string NextHref { get; set; }
    }
    public class PaginatedCollectionRegular
    {
        public Track[] Collection { get; set; }
        public string next_href { get; set; }
    }
    public class NextInfo
    {
        public string next_href { get; set; }
    }
    public class PlaylistRoot
    {
        public PlaylistItem[] PlaylistItems { get; set; }
    }
    public class PlaylistItem
    {
        public int duration { get; set; }
        public object release_day { get; set; }
        public string permalink_url { get; set; }
        public string genre { get; set; }
        public string permalink { get; set; }
        public object purchase_url { get; set; }
        public object release_month { get; set; }
        public object description { get; set; }
        public string uri { get; set; }
        public object label_name { get; set; }
        public string tag_list { get; set; }
        public object release_year { get; set; }
        public int track_count { get; set; }
        public int user_id { get; set; }
        public string last_modified { get; set; }
        public string license { get; set; }
        public Track[] tracks { get; set; }
        public object playlist_type { get; set; }
        public int id { get; set; }
        public bool? downloadable { get; set; }
        public string sharing { get; set; }
        public string created_at { get; set; }
        public object release { get; set; }
        public string kind { get; set; }
        public string title { get; set; }
        public object type { get; set; }
        public object purchase_title { get; set; }
        public Created_With created_with { get; set; }
        public object artwork_url { get; set; }
        public object ean { get; set; }
        public bool? streamable { get; set; }
        public User user { get; set; }
        public string embeddable_by { get; set; }
        public object label_id { get; set; }
    }
    public class TrackCreatedWith
    {
        public int id { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string external_url { get; set; }
    }

    public class Created_With
    {
        public string permalink_url { get; set; }
        public string name { get; set; }
        public string external_url { get; set; }
        public string uri { get; set; }
        public string creator { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
    }
    public class User
    {
        public string permalink_url { get; set; }
        public string permalink { get; set; }
        public string username { get; set; }
        public string uri { get; set; }
        public string last_modified { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
        public string avatar_url { get; set; }
    }

    public class Label
    {
        public int id { get; set; }
        public string kind { get; set; }
        public string permalink { get; set; }
        public string username { get; set; }
        public string last_modified { get; set; }
        public string uri { get; set; }
        public string permalink_url { get; set; }
        public string avatar_url { get; set; }
    }
}