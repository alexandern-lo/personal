namespace LiveOakApp.Models.Data.Entities
{
    public class CardPhone
    {
        public enum PhoneType
        {
            Home,
            Work,
            Mobile
        }

        public string Phone { get; set; }

        public PhoneType Type { get; set; }
    }
}
