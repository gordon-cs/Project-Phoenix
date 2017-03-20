using System.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Drawing;
using System.Collections.Generic;
using System;

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

        public IEnumerable<SignAllRDViewModel> GetRcisForBuildingThatCanBeSignedByRD(List<string> buildingCode)
        {
            // Not sure if this will end up with duplicates for the RA's own RCI
            var buildingRCIs =
                from r in db.Rci
                join account in db.Account on r.GordonID equals account.ID_NUM into rci
                from account in rci.DefaultIfEmpty()
                where buildingCode.Contains(r.BuildingCode) && r.IsCurrent == true
                where r.CheckinSigRD == null && r.CheckinSigRA != null && r.CheckinSigRes != null
                select new SignAllRDViewModel
                {
                    RciID = r.RciID,
                    BuildingCode = r.BuildingCode.Trim(),
                    RoomNumber = r.RoomNumber.Trim(),
                    FirstName = account.firstname == null ? "Common Area" : account.firstname,
                    LastName = account.lastname == null ? "RCI" : account.lastname,
                    CheckinSigRDGordonID = r.CheckinSigRDGordonID
                };
            return buildingRCIs.OrderBy(m => m.RoomNumber);
        }

        /*public IEnumerable<SignAllRDViewModel> GetCheckedRcis(string gordonID)
        {
            var rcis =
                from r in db.Rci
                where r.CheckinSigRDGordonID == gordonID
                where r.CheckinSigRD == null
                join a in db.Account on r.GordonID equals a.ID_NUM into account
                from temp in account.DefaultIfEmpty()
                select new SignAllRDViewModel()
                {
                    RciID = r.RciID,
                    GordonID = r.GordonID,
                    FirstName = temp.firstname ?? "Common Area",
                    LastName = temp.lastname ?? "Rci",
                    BuildingCode = r.BuildingCode,
                    RoomNumber = r.RoomNumber
                };
                
            return rcis;
        }*/

        /// <summary>
        /// Get the rci for a common area by id
        /// </summary>
        public CheckinCommonAreaRciViewModel GetCommonAreaRciById(int id)
        {
            var currentSession = new DashboardService().GetCurrentSession();

            var query =
                from rci in db.Rci
                where rci.RciID == id
                select new CheckinCommonAreaRciViewModel
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
                                                              && sigs.SignatureType == "CHECKIN"
                                                              select sigs).Any() == true ? true : false),
                                             Signature =
                                                             ((from sigs in db.CommonAreaRciSignature
                                                               where sigs.GordonID == acct.ID_NUM
                                                               && sigs.RciID == rci.RciID
                                                               && sigs.SignatureType == "CHECKIN"
                                                               select sigs).FirstOrDefault().Signature)
                                         }).ToList(),
                    CheckinSigRes = rci.CheckinSigRes,
                    CheckinSigRA = rci.CheckinSigRA,
                    CheckinSigRD = rci.CheckinSigRD,
                    CheckinSigRAGordonID = rci.CheckinSigRAGordonID,
                    CheckinSigRDGordonID = rci.CheckinSigRDGordonID,
                    CheckinSigRAName =
                                        (from acct in db.Account
                                         where acct.ID_NUM.Equals(rci.CheckinSigRAGordonID)
                                         select acct.firstname + " " + acct.lastname).FirstOrDefault(),
                    CheckinSigRDName =
                                         (from acct in db.Account
                                          where acct.ID_NUM.Equals(rci.CheckinSigRDGordonID)
                                          select acct.firstname + " " + acct.lastname).FirstOrDefault()

                };

            return query.FirstOrDefault();
        }
        public void SignRcis(string gordonID)
        {
            var rcis =
                from r in db.Rci
                where r.CheckinSigRDGordonID == gordonID
                where r.CheckinSigRD == null
                select r;
            foreach (var rci in rcis)
            {
                rci.CheckinSigRD = DateTime.Today;
            }
            db.SaveChanges();
        }

        public string GetName(string gordonID)
        {
            return db.Account.Where(m => m.ID_NUM == gordonID)
                    .Select(m => m.firstname + " " + m.lastname).FirstOrDefault();
        }

        public bool SaveCommonAreaMemberSig(string rciSig, string user, string gordonID, int rciID)
        {
            var rci = GetCommonAreaRciById(rciID);
            var alreadySigned = rci.CommonAreaMember.Where(m => m.GordonID == gordonID).FirstOrDefault().HasSignedCommonAreaRci;
            if (rciSig == user)
            {
                if(!alreadySigned)
                {
                    var signature = new CommonAreaRciSignature
                    {
                        RciID = rciID,
                        GordonID = gordonID,
                        Signature = DateTime.Now,
                        SignatureType = "CHECKIN"
                    };
                    db.CommonAreaRciSignature.Add(signature);
                    db.SaveChanges();
                }

                rci = GetCommonAreaRciById(rciID); // Check to see if everyone has signed now.
                if(rci.EveryoneHasSigned())
                {
                    var genericRci = db.Rci.Find(rciID);
                    if(genericRci.CheckinSigRes == null)
                    {
                        genericRci.CheckinSigRes = DateTime.Now;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveResSigs(string rciSig, string lacSig, string user, int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();

            if (rciSig == user)
            {
                rci.CheckinSigRes = DateTime.Today;
            }
            if (lacSig == user)
            {
                rci.LifeAndConductSigRes = DateTime.Today;
            }
            db.SaveChanges();
            return rci.CheckinSigRes != null && rci.LifeAndConductSigRes != null;
        }

        public bool SaveRASigs(string rciSig, string lacSig, string rciSigRes, string user, int id, string gordonID)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();

            if (rciSig == user || rci.GordonID == gordonID) // rciSig is null when it is the RA signing for himself. Make another check to see if the logged in RA is the owner of the rci.
            {
                rci.CheckinSigRA = DateTime.Today;
                rci.CheckinSigRAGordonID = gordonID;
            }
            if (rciSigRes == user)
            {
                rci.CheckinSigRes = DateTime.Today;
            }
            if (lacSig == user)
            {
                rci.LifeAndConductSigRes = DateTime.Today;
            }
            db.SaveChanges();
            return rci.CheckinSigRes != null && rci.LifeAndConductSigRes != null && rci.CheckinSigRA != null;
        }

        public bool SaveRDSigs(string rciSig, string user, int id, string gordonID)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            if (rciSig == user)
            {
                rci.CheckinSigRD = DateTime.Today;
                rci.CheckinSigRDGordonID = gordonID;
            }
            db.SaveChanges();
            return rci.CheckinSigRDGordonID != null;
        }

        public void CheckRcis(int sigCheck, string gordonID, int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            if (sigCheck == 1)
            {
                rci.CheckinSigRDGordonID = gordonID;
            }
            else
            {
                rci.CheckinSigRDGordonID = null;
            }
            db.SaveChanges();
        }


        public string GetUsername(string gordon_id)
        {
            var username = db.Account.Where(u => u.ID_NUM == gordon_id).FirstOrDefault().AD_Username;
            return username;
        }


        // Image resize method, taken mostly from  http://www.advancesharp.com/blog/1130/image-gallery-in-asp-net-mvc-with-multiple-file-and-size
        // Takes an original size, and returns proportional dimensions to be used in resizing the image
        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            //double tempval;
            int origHeight = imageSize.Height;
            int origWidth = imageSize.Width;
           

            if (origHeight > newSize.Height || origWidth > newSize.Width)
            {
                // Calculate the resizing ratio based on whichever is bigger - original height or original width
                decimal tempVal = origHeight > origWidth ? decimal.Divide(newSize.Height, origHeight) : decimal.Divide(newSize.Width, origWidth);
                //if (imageSize.Height > imageSize.Width)
                //    tempval = newSize.Height / (imageSize.Height * 1.0);
                //else
                //    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempVal * imageSize.Width), (int)(tempVal * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        public IEnumerable<string> GetCommonRooms(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci.RciComponent.GroupBy(x => x.RciComponentDescription).Select(x => x.First()).Select(x => x.RciComponentDescription);
        }

    }
}