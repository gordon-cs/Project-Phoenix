using Phoenix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Services
{
    public class RciReviewCheckinService
    {
        private RCIContext db;

        public RciReviewCheckinService()
        {
            db = new RCIContext();
        }

        public Rci GetRciByID(int rciID)
        {
            return db.Rci.Find(rciID);
        }
    }
}