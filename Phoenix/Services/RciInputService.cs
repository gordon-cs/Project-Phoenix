using System.Linq;
using Phoenix.Models.ViewModels;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Drawing.Drawing2D;
using Phoenix.DapperDal;
using Phoenix.Utilities;
using Phoenix.Exceptions;

namespace Phoenix.Services
{
    public class RciInputService : IRciInputService
    {
        private readonly IDatabaseDal Dal;

        private readonly IRciBatchService RciBatchService;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RciInputService(IDatabaseDal dal, IDashboardService dashboardService, IRciBatchService batchService)
        {
            this.Dal = dal;

            this.RciBatchService = batchService;
        }

        public FullRciViewModel GetRci(int id)
        {
            try
            {
                logger.Debug($"Fetching rci {id}...");

                var rci = this.Dal.FetchRciById(id);

                return new FullRciViewModel(rci);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while fetching rci with id {id}...");

                throw;
            }
        }

        public IEnumerable<SignAllRDViewModel> GetRcisForBuildingThatCanBeSignedByRD(string gordonId, List<string> buildingCodes)
        {
            try
            {
                logger.Debug($"Fetching Rcis that can be currently signed by the RD user {gordonId} ... ");

                // Fetch the rcis that can be signed by the Rd at this moment
                var rcis = this.Dal.FetchRcisByBuilding(buildingCodes)
                    .Where(x => x.IsCurrent)
                    .Where(x => x.ResidentCheckinDate != null && x.RaCheckinDate != null && x.RdCheckinDate == null);

                logger.Debug($"Fetching the rcis that the RD user {gordonId} has queued up for signing...");

                // Fetch the rcis that the rd has queued up for signing.
                var queuedRcisToBeSigned = this.RciBatchService.GetRcis(gordonId);

                logger.Debug($"Rcis that can be signed by the RD user {gordonId}: {rcis.Count()}...");

                logger.Debug($"Rcis that have been queued by the RD user {gordonId}: {queuedRcisToBeSigned.Count()}");

                var result = rcis
                    .Select(x =>
                    {
                        var isQueued = queuedRcisToBeSigned.Contains(x.RciId);

                        return new SignAllRDViewModel(x, isQueued);
                    })
                    .OrderBy(m => m.RoomNumber);

                return result;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while fetching rcis that RD user {gordonId} can sign at the moment");

                throw;
            }
        }

        public void SignRcis(string gordonID)
        {
            try
            {
                logger.Debug($"User {gordonID} has requested to sign all queued Rcis...");

                var rciIds = this.RciBatchService.GetRcis(gordonID);

                logger.Debug($"The following Rcis were found in user {gordonID}'s batch: {string.Join(",", rciIds)}");

                this.Dal.SetRciCheckinDateColumns(rciIds, null, null, DateTime.Today, null);

                this.Dal.SetRciCheckinGordonIdColumns(rciIds, null, gordonID);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected error occured while trying to sign rcis in batch. GordonId={gordonID}");

                throw;
            }
        }

        public bool SaveCommonAreaMemberSig(string rciSig, string user, string gordonID, int rciID)
        {
            try
            {
                logger.Debug($"A common area member requested to sign an rci for checkin. RciId={rciID}, GordonId={gordonID}, User={user}, Signature={rciSig}");

                if (rciSig == user)
                {
                    this.Dal.CreateNewCommonAreaRciSignature(gordonID, rciID, DateTime.Now, Constants.CHECKIN);

                    var rci = this.Dal.FetchRciById(rciID); // Check to see if everyone has signed now.

                    var apartmentMemberGordonIds = rci.RoomOrApartmentResidents
                        .Select(x => x.GordonId)
                        .OrderBy(x => x);

                    var residentsWhoHaveCheckedIn = rci.CommonAreaSignatures
                        .Where(x => x.SignatureType.Equals(Constants.CHECKIN) && x.SignatureDate != null)
                        .Select(x => x.GordonId)
                        .OrderBy(x => x);

                    logger.Debug($"User {gordonID} has signed. There are {apartmentMemberGordonIds.Count()} people in the apartment. {residentsWhoHaveCheckedIn.Count()} of whom have signed.");

                    bool everyoneHasSigned = residentsWhoHaveCheckedIn.SequenceEqual(apartmentMemberGordonIds);

                    if (everyoneHasSigned)
                    {
                        if (rci.ResidentCheckinDate == null)
                        {
                            logger.Debug($"Everyone in the apartment of user {gordonID} has signed! Updating the resident checkin column...");

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
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception submitting a common area signature....");

                throw;
            }
        }

        public bool SaveResSigs(string rciSig, string lacSig, string user, int id)
        {
            try
            {
                logger.Debug($"User {user} has requested to sign his checkin rci. RciId={id}, Signature={rciSig}, LifeAndConductStatementSignatures={lacSig}");

                var today = DateTime.Today;

                if (rciSig == user && lacSig == user)
                {
                    this.Dal.SetRciCheckinDateColumns(new List<int> { id }, today, null, null, today);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while trying to sign individual rci. User={user}, RciId={id}, RciSig={rciSig}, LacSig={lacSig}");

                throw;
            }
        }

        public bool SaveRASigs(string rciSig, string lacSig, string rciSigRes, string user, int id, string gordonID)
        {
            try
            {
                logger.Debug($"RA user {user} with gordonId {gordonID} is requesting to sign an Rci. RciSignature={rciSig}, RciSignatureRes={rciSigRes}");

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

                rci = this.Dal.FetchRciById(id);

                // If it is a common area rci, don't look for the life and conduct statment signature.
                if (rci.GordonId == null)
                {
                    return rci.ResidentCheckinDate != null && rci.RaCheckinDate != null;
                }
                // If it is not a common area rci, look for the life and conduct signature.
                else
                {
                    return rci.ResidentCheckinDate != null && rci.LifeAndConductSignatureDate != null && rci.RaCheckinDate != null;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while signing an rci as RD user {gordonID}");

                throw;
            }
        }

        public bool SaveRDSigs(string rciSig, string user, int id, string gordonID)
        {
            try
            {
                logger.Debug($"RD user {user}, with gordon Id {gordonID} is about to sign rci {id}. RciSig={rciSig}...");

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
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected error while signing rcis as RD user {user}, with gordonId = {gordonID}");

                throw;
            }
        }

        public void CheckRcis(int sigCheck, string gordonID, int id)
        {
            try
            {
                // Add this rci to the list that need to be signed.
                if (sigCheck == 1)
                {
                    logger.Debug($"Adding Rci {id} to rci batch for userId {gordonID}...");

                    this.RciBatchService.AddRciToBatch(gordonID, id);
                }
                else
                {
                    logger.Debug($"Removing Rci {id} from rci batch for userId {gordonID}...");

                    this.RciBatchService.RemoveRciFromBatch(gordonID, id);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected error while adding or removing rci {id} to user {gordonID}'s rci batch.");

                throw;
            }
        }

        public string FetchDamageFilePath(int damageId)
        {
            try
            {
                logger.Debug($"Fetching damage {damageId}...");

                var damage = this.Dal.FetchDamageById(damageId);

                return damage.ImagePath;
            }
            catch (DamageNotFoundException e)
            {
                logger.Error(e, $"Damage {damageId} does not exist!");

                throw;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while getting the damage file path for damage {damageId}...");

                throw;
            }
        }

        public int SaveTextDamage(string description, int rciId, string gordonId, int roomComponentTypeId)
        {
            try
            {
                return this.Dal.CreateNewDamage(description, null, rciId, gordonId, roomComponentTypeId);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while saving damage. GordonId={gordonId}, RciId={rciId}, RoomComponentTypeId={roomComponentTypeId}");

                throw;
            }
        }

        public int SavePhotoDamage(string imagePath, int rciId, string gordonId, int roomComponentTypeId)
        {
            try
            {
                return this.Dal.CreateNewDamage(null, imagePath, rciId, gordonId, roomComponentTypeId);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while saving damage. GordonId={gordonId}, RciId={rciId}, RoomComponentTypeId={roomComponentTypeId}");

                throw;
            }
        }

        public void DeleteDamage(int damageId)
        {
            try
            {
                this.Dal.DeleteDamage(damageId);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception while deleting damage {damageId}...");
                
                throw;
            }
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