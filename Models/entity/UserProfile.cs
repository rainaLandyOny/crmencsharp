namespace crmcsharp.Models.entity
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public decimal? Salary { get; set; } // Nullable
        public string Status { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Youtube { get; set; }
        public string OathUserImageLink { get; set; }
        public string UserImage { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
    }
}