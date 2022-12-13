using System.ComponentModel.DataAnnotations.Schema;

namespace UserLogin.Entity
{
    public class User
    {
        [Column(name: "user_id")]
        public int Id { get; set; }
        
        [Column(name: "user_name")]
        public string Name { get; set; }

        [Column(name: "email")]
        public string Email { get; set; }

        [Column(name: "user_role")]
        public string Role { get; set; }

        [Column(name: "passhash")]
        public string PasswordHash { get; set; }

        [Column(name: "activated")]
        public bool Activated { get; set; }
    }
}