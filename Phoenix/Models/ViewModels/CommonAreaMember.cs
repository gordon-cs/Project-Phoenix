using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Models.ViewModels
{
    public class CommonAreaMember
    {
        public string GordonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool HasSignedCommonAreaRci { get; set; }
        public DateTime? Signature { get; set; }
    }
}