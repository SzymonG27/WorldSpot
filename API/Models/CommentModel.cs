namespace API.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
    }
}
