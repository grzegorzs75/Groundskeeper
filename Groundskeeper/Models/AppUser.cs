namespace Groundskeeper.Models
{
    public enum UserGroup
    {
        Admin = 1,
        Employee = 2,
        Customer = 3
    }

   
    public class AppUser
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public UserGroup UserGroup { get; set; }

        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public bool Active { get; set; } = true;
    }
}
