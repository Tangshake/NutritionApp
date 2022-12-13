using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsCatalog.Entities
{
    [Table("product")]
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("name")]
        public string Name { get; set; }
        
        [Column("manufacturer")]
        public string Manufacturer { get; set; }
        
        [Required]
        [Column("kcal")]
        public int Kcal { get; set; }
        
        [Required]
        [Column("protein")]
        public float Protein { get; set; }
        
        [Required]
        [Column("fat")]
        public float Fat { get; set; }
        
        [Required]
        [Column("carbohydrates")]
        public float Carbohydrates { get; set; }
        
        [Required]
        [Column("roughage")]
        public float Roughage { get; set; }
    }
}