namespace InteriorBackend.Models
{
    public class UserSave
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<string> ImageUrl { get; set; }
    }
}
