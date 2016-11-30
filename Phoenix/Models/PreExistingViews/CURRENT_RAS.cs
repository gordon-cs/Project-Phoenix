namespace Phoenix.Models.PreExistingViews
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CURRENT_RAS
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_NUM { get; set; }

        [StringLength(15)]
        public string firstname { get; set; }

        [StringLength(16)]
        public string lastname { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string dorm { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int academic_year { get; set; }

        [StringLength(50)]
        public string email { get; set; }
    }
}
