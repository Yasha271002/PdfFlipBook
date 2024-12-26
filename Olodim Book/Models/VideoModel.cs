using Newtonsoft.Json;

namespace PdfFlipBook.Models
{
    public class VideoModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("video")]
        public string Video { get; set; }
    }
}