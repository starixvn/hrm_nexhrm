namespace Fanex.NexHR.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GeoMapInfo
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [Required]
        public string FullName { get; set; }

        public string NickName { get; set; }

        public string GeoMapLatitude { get; set; }

        public string GeoMapLongtitude { get; set; }

    }
}