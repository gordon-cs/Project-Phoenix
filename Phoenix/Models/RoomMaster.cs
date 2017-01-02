namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoomMaster")]
    public partial class RoomMaster
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int APPID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string LOC_CDE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string BLDG_CDE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(5)]
        public string ROOM_CDE { get; set; }

        [StringLength(45)]
        public string ROOM_DESC { get; set; }

        [StringLength(2)]
        public string ROOM_TYPE { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ROOM_PHONE { get; set; }

        [StringLength(5)]
        public string MAIL_STOP { get; set; }

        [StringLength(2)]
        public string AUDIO_RESOURCES { get; set; }

        [StringLength(2)]
        public string VIDEO_RESOURCES { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(1)]
        public string HANDICAP_ACCESS { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(1)]
        public string AVAIL_FOR_CLASSES { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAX_CAPACITY { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NUM_OF_COMPUTERS { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NUM_OF_CHAIRS { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NUM_OF_TABLES { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(1)]
        public string LAB_CLASSROOM { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(1)]
        public string CLASSROOM_W_WATER { get; set; }

        [StringLength(2)]
        public string FLOOR_TYPE { get; set; }

        [StringLength(10)]
        public string ROOM_DIMENSIONS { get; set; }

        [StringLength(10)]
        public string WINDOW_DIMENSIONS { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NET_SQUARE_FEET { get; set; }

        [StringLength(30)]
        public string COMMENT_TXT { get; set; }

        [Key]
        [Column(Order = 13)]
        [StringLength(1)]
        public string RESIDENCE_ROOM { get; set; }

        [StringLength(1)]
        public string ROOM_GENDER { get; set; }

        public int? ID_NUM_RM_CNTCT { get; set; }

        public int? RM_WHICH_FLOOR { get; set; }

        [StringLength(4)]
        public string RM_RSTRM_AVAIL { get; set; }

        [Key]
        [Column(Order = 14, TypeName = "numeric")]
        public decimal RM_RENTAL { get; set; }

        public string RM_NOTES { get; set; }

        [StringLength(2)]
        public string NETWORK_CONNECTION { get; set; }

        [StringLength(1)]
        public string UDEF_1A_1 { get; set; }

        [StringLength(1)]
        public string UDEF_1A_2 { get; set; }

        [StringLength(2)]
        public string UDEF_2A_1 { get; set; }

        [StringLength(2)]
        public string UDEF_2A_2 { get; set; }

        [StringLength(3)]
        public string UDEF_3A_1 { get; set; }

        [StringLength(3)]
        public string UDEF_3A_2 { get; set; }

        [StringLength(5)]
        public string UDEF_5A_1 { get; set; }

        [StringLength(5)]
        public string UDEF_5A_2 { get; set; }

        [Key]
        [Column(Order = 15, TypeName = "numeric")]
        public decimal UDEF_5_2_1 { get; set; }

        [Key]
        [Column(Order = 16, TypeName = "numeric")]
        public decimal UDEF_7_2_1 { get; set; }

        public int? UDEF_ID_1 { get; set; }

        [Key]
        [Column(Order = 17, TypeName = "timestamp")]
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
