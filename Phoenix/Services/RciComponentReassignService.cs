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

            var account = db.Account.Where(m => m.ID_NUM == gordonID).FirstOrDefault();
            firstName = account.firstname;
            lastName = account.lastname;

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
        public void SwapRciComponents(int[] rciComponents, int destinationRciID, int sourceRciID)
        {
            if(rciComponents == null)
            {
                return;
            }

            // The rci components to move from the source to the destination
            var query = db.RciComponent.Where(m => rciComponents.Contains(m.RciComponentID)).ToList();

            // THe names of the rci components to swap
            var temp = query.Select(m => m.RciComponentName);

            // The rci components to move from the destination back to the source
            var mirrorQuery = db.RciComponent.Where(m => m.RciID == destinationRciID && temp.Contains(m.RciComponentName)).ToList();

            foreach(var record in query)
            {
                
                record.RciID = destinationRciID;
            }

            foreach (var record in mirrorQuery)
            {
                record.RciID = sourceRciID;
            }

            db.SaveChanges();
        }


    }
}