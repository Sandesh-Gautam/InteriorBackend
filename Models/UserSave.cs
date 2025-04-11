using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace InteriorBackend.Models
{
    public class UserSave
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        [NotMapped]
        public List<string> ImageUrl { get; set; }

        public string ImageUrlsJson
        {
            get
            {
                return JsonSerializer.Serialize(ImageUrl);
            }

            set => ImageUrl = JsonSerializer.Deserialize<List<string>>(value ?? "[]");
        }
    }
}
