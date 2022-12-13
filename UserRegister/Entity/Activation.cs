using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRegister.Entity
{
    public class Activation
    {
        [Column(name: "id")]
        public int Id { get; set;
         }
        [Column(name: "nutuser_id")]
        public int UserId { get; set; }

        [Column(name: "email_token")]
        public string Token { get; set; }

        [Column(name: "activation_start")]
        public DateTime ActivationStart { get; set; }

        [Column(name: "activation_expiry")]
        public DateTime ActivationExpiry { get; set; }
    }
}