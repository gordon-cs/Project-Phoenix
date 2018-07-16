using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.DapperDal.Types
{
    public class Account
    {
        public string GordonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdUsername { get; set; }
        public string Email { get; set; }
        public bool IsRa { get; set; }
        public string RaBuildingCode { get; set; }
        public bool IsRd { get; set; }
        public string RdHallGroup { get; set; }
        public bool IsAdmin { get; set; }
    }
}