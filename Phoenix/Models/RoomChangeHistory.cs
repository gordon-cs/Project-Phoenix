namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoomChangeHistory")]
    public partial class RoomChangeHistory
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string SESS_CDE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_NUM { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime ROOM_CHANGE_DTE { get; set; }

        [StringLength(5)]
        public string OLD_BLDG_LOC_CDE { get; set; }

        [StringLength(5)]
        public string OLD_BLDG_CDE { get; set; }

        [StringLength(5)]
        public string OLD_ROOM_CDE { get; set; }

        [StringLength(5)]
        public string NEW_BLDG_LOC_CDE { get; set; }

        [StringLength(5)]
        public string NEW_BLDG_CDE { get; set; }

        [StringLength(5)]
        public string NEW_ROOM_CDE { get; set; }

        [StringLength(3)]
        public string ROOM_CHANGE_REASON { get; set; }

        [StringLength(255)]
        public string ROOM_CHANGE_COMMENT { get; set; }

        [StringLength(15)]
        public string USER_NAME { get; set; }

        [StringLength(30)]
        public string JOB_NAME { get; set; }

        public DateTime? JOB_TIME { get; set; }
    }
}
