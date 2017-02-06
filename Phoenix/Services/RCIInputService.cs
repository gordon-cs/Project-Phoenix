using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public class RCIInputService
    {
        private RCIContext db;
        public RCIInputService()
        {
            db = new Models.RCIContext();
        }

        public RCI GetRCI(int id)
        {
            var rci = db.RCI.Where(m => m.RCIID == id).FirstOrDefault();
            return rci;
        }

        public string GetUsername(string gordon_id)
        {
            var username = db.Account.Where(u => u.ID_NUM == gordon_id).FirstOrDefault().AD_Username;
            return username;
        }
    }
}