using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Services
{
    public class RciComponentReassignService
    {
        private RCIContext db;
        public RciComponentReassignService()
        {
            db = new RCIContext();
        }

        public RciReassignViewModel GetRciByID(int id)
        {
            var temp = db.Rci.Find(id);
            var gordonID = temp.GordonID;
            string firstName, lastName;

            if (gordonID != null)
            {
                var account = db.Account.Where(m => m.ID_NUM == gordonID).FirstOrDefault();
                firstName = account.firstname;
                lastName = account.lastname;
            }
            else
            {
                firstName = "Common Area";
                lastName = "RCI";
            }

            var rci = new RciReassignViewModel()
            {
                RciID = temp.RciID,
                GordonID = gordonID,
                FirstName = firstName,
                LastName = lastName,
                BuildingCode = temp.BuildingCode,
                RoomNumber = temp.RoomNumber,
                RciComponent = temp.RciComponent
            };

            // Find the rcis for the room mates, since those are the only people you can reassign 
            // components to.
            var roommateRcis =
                from r in db.Rci
                join a in db.Account on r.GordonID equals a.ID_NUM
                where r.IsCurrent == true &&
                r.BuildingCode == rci.BuildingCode &&
                r.RoomNumber == rci.RoomNumber
                select new PotentialRciReassignTarget
                {
                    RciID = r.RciID,
                    RciOwner = a.firstname + " " + a.lastname
                };

            rci.RciTarget = roommateRcis.ToList();

            return rci;
        }

        // Set the RciID column for a list components.
        public void AssignComponentsToRci(int[] rciComponents, int rciID)
        {
            var query = db.RciComponent.Where(m => rciComponents.Contains(m.RciComponentID));
            foreach(var record in query)
            {
                record.RciID = rciID;
            }

            db.SaveChanges();
        }


    }
}