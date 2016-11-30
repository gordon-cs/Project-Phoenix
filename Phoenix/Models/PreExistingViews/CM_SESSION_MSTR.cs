namespace Phoenix.Models.PreExistingViews
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CM_SESSION_MSTR
    {
        [Key]
        [StringLength(8)]
        public string SESS_CDE { get; set; }

        [StringLength(60)]
        public string SESS_DESC { get; set; }

        public DateTime? SESS_BEGN_DTE { get; set; }

        public DateTime? SESS_END_DTE { get; set; }
    }
}
