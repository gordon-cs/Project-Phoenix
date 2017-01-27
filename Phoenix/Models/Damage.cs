namespace Phoenix.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Damage")]
    public partial class Damage
    {
        public int DamageID { get; set; }

        public string DamageDescription { get; set; }

        public string DamageImagePath { get; set; }

        [Required]
        [StringLength(50)]
        public string DamageType { get; set; }

        public int RCIComponentID { get; set; }
    }
}
