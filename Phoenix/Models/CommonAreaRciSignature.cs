namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommonAreaRciSignature")]
    public partial class CommonAreaRciSignature
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string GordonID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RciID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Signature { get; set; }

        [Required]
        [StringLength(50)]
        public string SignatureType { get; set; }

        public virtual Rci Rci { get; set; }
    }
}
