using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using Phoenix.Models.PreExistingViews;

namespace Phoenix.Models
{
    /// <summary>
    /// A record representing a residen
    /// </summary>
    public class ResidentAccount
    {
        // Primary key
        [Key]
        public string ResidentAccountID { get; set; }

        public string Year { get; set; }
    }
}