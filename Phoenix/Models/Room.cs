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
        [StringLength(5)]
        public string ID_NUM { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string buildingCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string roomNumber { get; set; }

        [StringLength(45)]
        public string ROOM_DESC { get; set; }

        [StringLength(2)]
        public string ROOM_TYPE { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MAX_CAPACITY { get; set; }

        [StringLength(1)]
        public string ROOM_GENDER { get; set; }

        public int? RM_WHICH_FLOOR { get; set; }
    }
}
