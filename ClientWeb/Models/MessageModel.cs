namespace ClientWeb.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } //Żeby można było potem bezpośrednio z czatu wejść na profil użytkownika
        public string UserName { get; set; } //Żeby nie pobierać dodatkowo użytkowników z bazy w czacie
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ChatModelId { get; set; }
    }
}
