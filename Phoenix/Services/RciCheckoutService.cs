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
            db = new RCIContext();
        }

        public bool IsIndividualRci(int rciID)
        {
            var rci = db.Rci.Find(rciID);
            return rci.GordonID != null ? true : false;
        }
        /// <summary>
        /// Get the rci for a person by RciID
        /// </summary>
        public CheckoutIndividualRoomRciViewModel GetIndividualRoomRciByID(int id)
        {
            var query =
                from r in db.Rci
                join a in db.Account on r.GordonID equals a.ID_NUM
                where r.RciID == id
                select new CheckoutIndividualRoomRciViewModel()
                {
                    RciID = r.RciID,
                    GordonID = r.GordonID,
                    FirstName = a.firstname,
                    LastName = a.lastname,
                    BuildingCode = r.BuildingCode,
                    RoomNumber = r.RoomNumber,
                    RciComponent = r.RciComponent,
                    CheckoutSigRes = r.CheckoutSigRes,
                    CheckoutSigRA = r.CheckoutSigRA,
                    CheckoutSigRD = r.CheckoutSigRD,
                    CheckoutSigRAGordonID = r.CheckoutSigRAGordonID,
                    CheckoutSigRDGordonID = r.CheckoutSigRDGordonID,
                    CheckoutSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(r.CheckoutSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckoutSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(r.CheckoutSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault()

                };

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get the rci for a common area by ID
        /// </summary>
        public CheckoutCommonAreaRciViewModel GetCommonAreaRciByID(int id)
        {
            var currentSession = new DashboardService().GetCurrentSession();

            var query =
                from rci in db.Rci
                where rci.RciID == id
                select new CheckoutCommonAreaRciViewModel
                {
                    RciID = rci.RciID,
                    BuildingCode = rci.BuildingCode,
                    RoomNumber = rci.RoomNumber,
                    RciComponent = rci.RciComponent,
                    CommonAreaMember =
                                        (from rm in db.RoomAssign
                                         join acct in db.Account
                                         on rm.ID_NUM.ToString() equals acct.ID_NUM
                                         where rm.SESS_CDE.Trim() == "201701"
                                         && rm.BLDG_CDE.Trim() == rci.BuildingCode
                                         && rm.ROOM_CDE.Trim().Contains(rci.RoomNumber)
                                         select new CommonAreaMember
                                         {
                                             GordonID = acct.ID_NUM,
                                             FirstName = acct.firstname,
                                             LastName = acct.lastname,
                                             HasSignedCommonAreaRci = 
                                                            ((from sigs in db.CommonAreaRciSignature
                                                             where sigs.GordonID == rci.GordonID
                                                             && sigs.RciID == rci.RciID
                                                             select sigs).Any() == true ? true : false),
                                             Signature =
                                                             ((from sigs in db.CommonAreaRciSignature
                                                               where sigs.GordonID == rci.GordonID
                                                               && sigs.RciID == rci.RciID
                                                               select sigs).FirstOrDefault().Signature)
                                         }).ToList(),
                    CheckoutSigRes = rci.CheckoutSigRes,
                    CheckoutSigRA = rci.CheckoutSigRA,
                    CheckoutSigRD = rci.CheckoutSigRD,
                    CheckoutSigRAGordonID = rci.CheckoutSigRAGordonID,
                    CheckoutSigRDGordonID = rci.CheckoutSigRDGordonID,
                    CheckoutSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(rci.CheckoutSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckoutSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(rci.CheckoutSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault()

                };

            return query.FirstOrDefault();

        }

        /// <summary>
        /// Creates an RCI Component called Improper checkout and adds a fine
        /// </summary>
        public void SetImproperCheckout(int rciID)
        {
            var rci = db.Rci.Find(rciID);

            // Create a new component
            var comp = new RciComponent
            {
                RciComponentName = "Improper Checkout",
                RciID = rciID
            };

            if (!rci.RciComponent.Where(m => m.RciComponentName.Equals(comp.RciComponentName)).Any())
            {
                var newComponent = db.RciComponent.Add(comp);

                db.SaveChanges();

                var fine = new Fine
                {
                    FineAmount = 30.00M,
                    GordonID = db.Rci.Find(rciID).GordonID,
                    RciComponentID = newComponent.RciComponentID,
                    Reason = "Improper Checkout"
                };

                db.Fine.Add(fine);

                db.SaveChanges();
            }

        }

        /// <summary>
        /// Creates an RCI component called Lost Keys and adds a fine to it
        /// </summary>
        public void SetLostKeyFine(int rciID, decimal fineAmount)
        {
            var rci = db.Rci.Find(rciID);

            var comp = new RciComponent
            {
                RciComponentName = "Lost Keys",
                RciID = rciID
            };

            if (!rci.RciComponent.Where(m => m.RciComponentName.Equals(comp.RciComponentName)).Any())
            {
                var newComponent = db.RciComponent.Add(comp);

                db.SaveChanges();

                var fine = new Fine
                {
                    FineAmount = fineAmount,
                    GordonID = db.Rci.Find(rciID).GordonID,
                    RciComponentID = newComponent.RciComponentID,
                    Reason = "Lost Keys"
                };

                db.Fine.Add(fine);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Insert fines into the database
        /// </summary>
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

        /// <summary>
        /// Delete a list of fines from the database
        /// </summary>
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

        /// <summary>
        /// Sign the resident portion of the rci during the checkout process
        /// </summary>
        public void CheckoutResidentSignRci(CheckoutIndividualRoomRciViewModel rciViewModel)
        {
            var rci = db.Rci.Find(rciViewModel.RciID);

            rci.CheckoutSigRes = System.DateTime.Today;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RA portion of the rci during the checkout process
        /// </summary>
        public void CheckoutRASignRci(CheckoutIndividualRoomRciViewModel rciViewModel, string raName, string raGordonID)
        {
            var rci = db.Rci.Find(rciViewModel.RciID);

            rci.CheckoutSigRA = System.DateTime.Today;
            rci.CheckoutSigRAGordonID = raGordonID;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RD portion of the rci during the checkout process and make the rci non-current.
        /// </summary>
        public void CheckoutRDSignRci(CheckoutIndividualRoomRciViewModel rciViewModel, string rdName, string rdGordonID)
        {
            var rci = db.Rci.Find(rciViewModel.RciID);

            rci.CheckoutSigRD = System.DateTime.Today;
            rci.CheckoutSigRDGordonID = rdGordonID;
            rci.IsCurrent = false;

            db.SaveChanges();

        }

        public IEnumerable<string> GetCommonRooms(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci.RciComponent.GroupBy(x => x.RciComponentDescription).Select(x => x.First()).Select(x => x.RciComponentDescription);
        }

    }
}