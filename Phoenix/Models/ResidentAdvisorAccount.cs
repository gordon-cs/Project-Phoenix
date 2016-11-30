using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Phoenix.Models
{
    [Table("ResidentAdvisorAccount")]
    public class ResidentAdvisorAccount
    {
        [Key, ForeignKey("ResidentAccount")]
        public string ResidentAccountID { get; set; }

        public virtual ResidentAccount ResidentAccount { get; set; }
        
    }
}