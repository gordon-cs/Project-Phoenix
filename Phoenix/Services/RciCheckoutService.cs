using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;


namespace Phoenix.Services
{
    public class RciCheckoutService
    {
        private RCIContext db;
        public RciCheckoutService()
        {
            db = new Models.RCIContext();
        }

        public CheckoutRciViewModel GetRciByID(int id)
        {
            var temp = db.Rci.Find(id);
            var gordonID = temp.GordonID;
            string firstName, lastName;

            if(gordonID != null)
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

            var rci = new CheckoutRciViewModel()
            {
                RciID = temp.RciID,
                GordonID = gordonID,
                FirstName = firstName,
                LastName = lastName,
                BuildingCode = temp.BuildingCode,
                RoomNumber = temp.RoomNumber,
                RciComponent = temp.RciComponent
            };

            return rci;
        }

        public void AddFines(List<RciNewFineViewModel> newFines, string gordonID)
        {
            if (newFines != null)
            {
                var toAdd = new List<Fine>();

                foreach (var fine in newFines)
                {
                    var newFine = new Fine { RciComponentID = fine.ComponentID, Reason = fine.FineReason, FineAmount = fine.FineAmount, GordonID = gordonID };
                    toAdd.Add(newFine);
                }
                db.Fine.AddRange(toAdd);
            }

            db.SaveChanges();
        }

        public void RemoveFines(List<int> fineIDs)
        {
            if(fineIDs != null)
            {
                var toDelete = new List<Fine>();
                foreach (var fineID in fineIDs)
                {
                    var fine = db.Fine.Find(fineID);
                    toDelete.Add(fine);
                }
                db.Fine.RemoveRange(toDelete);
            }

            db.SaveChanges();
        }

    }
}