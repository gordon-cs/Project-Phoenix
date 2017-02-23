using System.Linq;
using Phoenix.Models;

namespace Phoenix.Services
{
    public class RciInputService
    {
        private RCIContext db;
        public RciInputService()
        {
            db = new Models.RCIContext();
        }

        public Rci GetRci(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci;
        }

        public string GetUsername(string gordon_id)
        {
            var username = db.Account.Where(u => u.ID_NUM == gordon_id).FirstOrDefault().AD_Username;
            return username;
        }

    }
}