using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTService.Entity
{
    public class JwtTokenEntity
    {
        [Column(name:"id")]
        public int Id { get; set; }
        [Column(name:"secret_key")]
        public string SecretKey { get; set; }
        [Column(name:"active")]
        public bool Active { get; set; }
        [Column(name:"activation_date")]
        public DateTime ActivationDate { get; set; }
        [Column(name:"deactivation_date")]
        public DateTime DeactivationDate { get; set; }
    }
}