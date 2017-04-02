using Phoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Services
{
    public class AdminDashboardService
    {
        private RCIContext db;

        public AdminDashboardService()
        {
            db = new Models.RCIContext();
        }
        
        /* Get a list of all current building codes in the system
         
        public IEnumerable<string> GetBuildings()
        {
            
        }*/
        

        /* Get a list of last 5 sessions
        public IEnumerable<string> GetSessions()
        {

        }
        */


    }
}