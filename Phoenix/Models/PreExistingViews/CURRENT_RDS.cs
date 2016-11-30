namespace Phoenix.Models.PreExistingViews
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CURRENT_RDS
    {
        [Key]
        [StringLength(10)]
        public string ID_NUM { get; set; }

        [StringLength(50)]
        public string firstname { get; set; }

        [StringLength(50)]
        public string lastname { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(512)]
        public string Job_Title { get; set; }

        [StringLength(512)]
        public string Job_Title_Hall { get; set; }
    }
}
