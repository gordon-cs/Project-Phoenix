namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CurrentRA")]
    public partial class CurrentRA
    {
        [StringLength(3)]
        public string Dorm { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string AD_Username { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_NUM { get; set; }
    }
}