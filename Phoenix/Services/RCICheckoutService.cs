using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;


namespace Phoenix.Services
{
    public class RCICheckoutService
    {
        private RCIContext db;
        public RCICheckoutService()
        {
            db = new Models.RCIContext();
        }

        public CheckoutRCIViewModel GetRCIByID(int id)
        {
            var temp = db.RCI.Find(id);
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

            var rci = new CheckoutRCIViewModel()
            {
                RCIID = temp.RCIID,
                gordonID = gordonID,
                firstName = firstName,
                lastName = lastName,
                buildingCode = temp.BuildingCode,
                roomNumber = temp.RoomNumber,
                rciComponents = temp.RCIComponent
            };

            return rci;
        }

        public void AddFines(List<RCInewFineViewModel> newFines, string gordonID)
        {
            if (newFines != null)
            {
                var toAdd = new List<Fine>();

                foreach (var fine in newFines)
                {
                    var newFine = new Fine { RCIComponentID = fine.componentId, Reason = fine.fineReason, FineAmount = fine.fineAmount, GordonID = gordonID };
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