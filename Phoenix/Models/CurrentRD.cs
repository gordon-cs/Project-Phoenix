namespace Phoenix.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CurrentRD")]
    public partial class CurrentRD
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
