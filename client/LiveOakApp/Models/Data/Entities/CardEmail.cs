namespace LiveOakApp.Models.Data.Entities
{
    public class CardEmail
    {
        public enum EmailType
        {
            Home,
            Work,
            Other
        }

        public string Email { get; set; }

        public EmailType Type { get; set; }
    }
}
