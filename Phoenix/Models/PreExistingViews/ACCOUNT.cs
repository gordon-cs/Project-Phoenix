namespace Phoenix.Models.PreExistingViews
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ACCOUNT")]
    public partial class ACCOUNT
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string ID_NUM { get; set; }

        [StringLength(50)]
        public string firstname { get; set; }

        [StringLength(50)]
        public string lastname { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string AD_Username { get; set; }

        [StringLength(20)]
        public string account_type { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Private { get; set; }
    }
}
