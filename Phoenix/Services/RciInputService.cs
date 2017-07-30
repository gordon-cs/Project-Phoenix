using System.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Drawing.Drawing2D;

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
            if (rciSig == user)
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

                rci = GetCommonAreaRciById(rciID); // Check to see if everyone has signed now.
                if(rci.EveryoneHasSigned())
                {
                    var genericRci = db.Rci.Find(rciID);
                    if(genericRci.CheckinSigRes == null)
                    {
                        genericRci.CheckinSigRes = DateTime.Now;
                        db.SaveChanges();
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
            // If it is a common area rci, don't look for the life and conduct statment signature.
            if(rci.GordonID == null)
            {
                return rci.CheckinSigRes != null  && rci.CheckinSigRA != null;
            }
            // If it is not a common area rci, look for the life and conduct signature.
            else
            {
                return rci.CheckinSigRes != null && rci.LifeAndConductSigRes != null && rci.CheckinSigRA != null;
            }
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

        // Save a damage to the Damage table in the db
        // @return the resulting image path that was saved to the db
        public void SavePhotoDamage(Damage damage, string rciComponentId)
        {
            db.Damage.Add(damage);
            db.SaveChanges();
        }

        // Create the correct path for a photo damage
        public void SaveImagePath(string fullPath, Damage damage)
        {
            damage.DamageImagePath = fullPath;

            db.SaveChanges();
        }


        // Get the new size for an image to be rescaled, taken mostly from  http://www.advancesharp.com/blog/1130/image-gallery-in-asp-net-mvc-with-multiple-file-and-size
        // Takes an original size, and returns proportional dimensions to be used in resizing the image
        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            int origHeight = imageSize.Height;
            int origWidth = imageSize.Width;
           

            if (origHeight > newSize.Height || origWidth > newSize.Width)
            {
                // Calculate the resizing ratio based on whichever is bigger - original height or original width
                decimal tempVal = origHeight > origWidth ? decimal.Divide(newSize.Height, origHeight) : decimal.Divide(newSize.Width, origWidth);

                finalSize = new Size((int)(tempVal * imageSize.Width), (int)(tempVal * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        public void ResizeImage(Image origImg, Image newImage, Size imgSize)
        {
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(origImg, new Rectangle(0, 0, imgSize.Width, imgSize.Height));
            }
        }

        /// <summary>
        /// Respect EXIF orientation metadata and apply it to the image.
        /// References:
        /// EXIF: http://www.impulseadventure.com/photo/exif-orientation.html 
        /// Modifying an image to reflext EXIF data: http://stackoverflow.com/questions/6222053/problem-reading-jpeg-metadata-orientation#23400751
        /// Extra :p http://www.daveperrett.com/articles/2012/07/28/exif-orientation-handling-is-a-ghetto/
        /// </summary>
        public void ApplyExifData(Image image)
        {
            if(!image.PropertyIdList.Contains(274))
            {
                return;
            }
            var orientation = image.GetPropertyItem(274).Value[0];
            switch (orientation)
            {
                case 1:
                    // No rotation required.
                    break;
                case 2:
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            // Now that we have the image rotated correctly, remove the exif information so that
            // other programs don't try to adjust the image again based on the exif data.
            image.RemovePropertyItem(274);
        } 

        public IEnumerable<string> GetCommonRooms(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci.RciComponent.GroupBy(x => x.RciComponentDescription).Select(x => x.First()).Select(x => x.RciComponentDescription);
        }

    }
}