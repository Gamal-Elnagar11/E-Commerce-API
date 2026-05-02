using System.Text.Json.Serialization;

namespace E_Commerce_API.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreatedAtFormatted => CreatedAt.ToString("yyyy-MM-dd HH:mm");

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }

    }
}
