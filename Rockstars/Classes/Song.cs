using System.ComponentModel.DataAnnotations;

namespace Rockstars.Classes
{
    public class Song
    {
        private string _name;
        private string _artist;
        private string _genre;
        private string _album;
        public int Id { get; set; }

        public string Name
        {
            get => _name.ToLower();
            set => _name = value.ToLower();
        }
        public int Year { get; set; }

        public string Artist
        {
            get => _artist.ToLower();
            set => _artist = value.ToLower();
        }
        public string Shortname { get; set; }
        public int? Bpm { get; set; }
        public int Duration { get; set; }
        public string Genre
        {
            get => _genre.ToLower();
            set => _genre = value.ToLower();
        }
        public string SpotifyId { get; set; }
        public string Album
        {
            get
            {
                if (_album != null)
                {
                    return _album.ToLower();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_album != null)
                {
                    _album = value.ToLower();
                }
            }
        }
    }
}
