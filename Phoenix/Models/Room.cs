namespace Phoenix.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Room")]
    public partial class Room
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

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAX_CAPACITY { get; set; }

        [StringLength(1)]
        public string ROOM_GENDER { get; set; }

        public int? RM_WHICH_FLOOR { get; set; }
    }
}
