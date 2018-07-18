using System.Linq;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Drawing.Drawing2D;
using Phoenix.DapperDal;
using Phoenix.Utilities;

namespace Phoenix.Services
{
    public class RciInputService : IRciInputService
    {
        private RCIContext db;

        private readonly IDal Dal;

        private IDashboardService DashboardService { get; set; }


        public RciInputService(IDal dal, IDashboardService dashboardService)
        {
            db = new Models.RCIContext();

            this.Dal = dal;

            this.DashboardService = dashboardService;
        }

        public FullRciViewModel GetRci(int id)
        {
            var rci =  this.Dal.FetchRciById(id);

            return new FullRciViewModel(rci);
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

        public bool SaveCommonAreaMemberSig(string rciSig, string user, string gordonID, int rciID)
        {
            var rci = this.Dal.FetchRciById(rciID);
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

                rci = this.Dal.FetchRciById(rciID); // Check to see if everyone has signed now.

                var apartmentMemberGordonIds = rci.RoomOrApartmentResidents
                    .Select(x => x.GordonId)
                    .OrderBy(x => x);

                var residentsWhoHaveCheckedIn = rci.CommonAreaSignatures
                    .Where(x => x.SignatureType.Equals(Constants.CHECKIN) && x.SignatureDate != null)
                    .Select(x => x.GordonId)
                    .OrderBy(x => x);

                bool everyoneHasSigned = residentsWhoHaveCheckedIn.SequenceEqual(apartmentMemberGordonIds);
                
                if(everyoneHasSigned)
                {
                    if(rci.ResidentCheckinDate == null)
                    {
                        this.Dal.SetRciCheckinDateColumns(new List<int> { rci.RciId }, DateTime.Today, null, null, null);
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
            var today = DateTime.Today;

            if (rciSig == user && lacSig == user)
            {
                this.Dal.SetRciCheckinDateColumns(new List<int> { id }, today, null, null, today);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool SaveRASigs(string rciSig, string lacSig, string rciSigRes, string user, int id, string gordonID)
        {
            var rci = this.Dal.FetchRciById(id);

            var idList = new List<int> { id };

            if (rciSig == user || rci.GordonId == gordonID) // rciSig is null when it is the RA signing for himself. Make another check to see if the logged in RA is the owner of the rci.
            {
                this.Dal.SetRciCheckinDateColumns(idList, null, DateTime.Today, null, null);
                this.Dal.SetRciCheckinGordonIdColumns(idList, gordonID, null);
            }
            if (rciSigRes == user)
            {
                this.Dal.SetRciCheckinDateColumns(idList, DateTime.Today, null, null, null);
            }
            if (lacSig == user)
            {
                this.Dal.SetRciCheckinDateColumns(idList, null, null, null, DateTime.Today);
            }
            db.SaveChanges();

            rci = this.Dal.FetchRciById(id);

            // If it is a common area rci, don't look for the life and conduct statment signature.
            if(rci.GordonId == null)
            {
                return rci.ResidentCheckinDate != null  && rci.RaCheckinDate != null;
            }
            // If it is not a common area rci, look for the life and conduct signature.
            else
            {
                return rci.ResidentCheckinDate != null && rci.LifeAndConductSignatureDate != null && rci.RaCheckinDate != null;
            }
        }

        public bool SaveRDSigs(string rciSig, string user, int id, string gordonID)
        {
            var rci = this.Dal.FetchRciById(id);

            var idList = new List<int> { id };

            if (rciSig == user)
            {
                this.Dal.SetRciCheckinDateColumns(idList, null, null, DateTime.Today, null);
                this.Dal.SetRciCheckinGordonIdColumns(idList, null, gordonID);
            }

            rci = this.Dal.FetchRciById(id);

            return rci.CheckinRdGordonId != null;
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
    }
}