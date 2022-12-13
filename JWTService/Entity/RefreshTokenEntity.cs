using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTService.Entity
{
    public class RefreshToken
    {
        [Column(name:"rid")]
        public int RId { get; set; }

        [Column(name:"user_id")]
        public int UserId { get; set; }
    
        [Column(name:"rtoken")]
        public string Token { get; set; }
        
        [Column(name:"activation_date")]
        public DateTime ActivationDate { get; set; }

        [Column(name:"deactivation_date")]
        public DateTime DeactivationDate { get; set; }
    }
}