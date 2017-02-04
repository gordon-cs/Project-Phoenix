namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoomAssign")]
    public partial class RoomAssign
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int APPID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string SESS_CDE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string BLDG_LOC_CDE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(5)]
        public string BLDG_CDE { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(5)]
        public string ROOM_CDE { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ROOM_SLOT_NUM { get; set; }

        public int? ID_NUM { get; set; }

        [StringLength(2)]
        public string ROOM_TYPE { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(1)]
        public string ROOM_ASSIGN_STS { get; set; }

        public DateTime? ASSIGN_DTE { get; set; }

        [Key]
        [Column(Order = 7, TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] APPROWVERSION { get; set; }

        [StringLength(513)]
        public string USER_NAME { get; set; }

        [StringLength(30)]
        public string JOB_NAME { get; set; }

        public DateTime? JOB_TIME { get; set; }
    }
}
