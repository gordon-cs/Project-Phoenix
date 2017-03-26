using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System;
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
        /// Get a generic checkout rci view model.
        /// The RA and RD views use this since they don't need the other information
        /// </summary>
        public GenericCheckoutViewModel GetGenericCheckoutRciByID(int id)
        {
            var query =
                from r in db.Rci
                where r.RciID == id
                select new GenericCheckoutViewModel()
                {
                    RciID = r.RciID,
                    BuildingCode = r.BuildingCode,
                    RoomNumber = r.RoomNumber,
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
                                         where rm.SESS_CDE.Trim() == currentSession
                                         && rm.BLDG_CDE.Trim() == rci.BuildingCode
                                         && rm.ROOM_CDE.Trim().Contains(rci.RoomNumber)
                                         select new CommonAreaMember
                                         {
                                             GordonID = acct.ID_NUM,
                                             FirstName = acct.firstname,
                                             LastName = acct.lastname,
                                             HasSignedCommonAreaRci =
                                                            ((from sigs in db.CommonAreaRciSignature
                                                              where sigs.GordonID == acct.ID_NUM
                                                              && sigs.RciID == rci.RciID
                                                              && sigs.SignatureType == "CHECKOUT"
                                                              select sigs).Any() == true ? true : false),
                                             Signature =
                                                             ((from sigs in db.CommonAreaRciSignature
                                                               where sigs.GordonID == acct.ID_NUM
                                                               && sigs.RciID == rci.RciID
                                                               && sigs.SignatureType == "CHECKOUT"
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
        /// Add a fine record to the database
        /// </summary>
        public int AddFine(RciNewFineViewModel newFine)
        {
            var obj = new Fine { RciComponentID = newFine.ComponentID, Reason = newFine.FineReason, FineAmount = newFine.FineAmount, GordonID = newFine.FineOwner };
            db.Fine.Add(obj);
            db.SaveChanges();
            return obj.FineID;
        }

        /// <summary>
        /// Delete a fine from the database
        /// </summary>
        public void RemoveFine(int fineID)
        {
            var fine = db.Fine.Find(fineID);
            db.Fine.Remove(fine);
            db.SaveChanges();
        }

        /// <summary>
        /// Let the indicated resident sign the rci. This signature is represented by adding a record to the
        /// CommonAreaRciSignature table.
        /// </summary>
        public void CheckoutCommonAreaMemberSignRci(int rciID, string gordonID)
        {
            var signature = new CommonAreaRciSignature
            {
                RciID = rciID,
                GordonID = gordonID,
                Signature = DateTime.Now,
                SignatureType = "CHECKOUT"
            };
            db.CommonAreaRciSignature.Add(signature);
            db.SaveChanges();
        }
        /// <summary>
        /// Sign the resident portion of the rci during the checkout process
        /// </summary>
        public void CheckoutResidentSignRci(int rciID)
        {
            var rci = db.Rci.Find(rciID);

            rci.CheckoutSigRes = System.DateTime.Today;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RA portion of the rci during the checkout process
        /// </summary>
        public void CheckoutRASignRci(int rciID, string raGordonID)
        {
            var rci = db.Rci.Find(rciID);

            rci.CheckoutSigRA = System.DateTime.Today;
            rci.CheckoutSigRAGordonID = raGordonID;

            db.SaveChanges();

        }

        /// <summary>
        /// Sign the RD portion of the rci during the checkout process and make the rci non-current.
        /// </summary>
        public void CheckoutRDSignRci(int rciID, string rdGordonID)
        {
            var rci = db.Rci.Find(rciID);

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