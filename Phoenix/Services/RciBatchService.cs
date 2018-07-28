using Newtonsoft.Json;
using Phoenix.FileSystemDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Services
{
    /// <summary>
    /// A simple service to encapsulate the logic for letting a user flag a list of rcis.
    /// Currently only used to let an RD flag a list of rcis for signature, but could potentially be used in multiple other ways.
    /// </summary>
    public class RciBatchService : IRciBatchService
    {
        private readonly IFileSystemDal FsDal;

        private readonly string RootFolder;

        private const string RciBatchesFolder = "RciBatches";

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RciBatchService(IFileSystemDal fsDal)
        {
            this.FsDal = fsDal;

            this.RootFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
        }

        // Add this rciId to the batch belonging to this gordonId. If it already exists, nothing happends.
        public void AddRciToBatch(string gordonId, int rciId)
        {
            try
            {
                logger.Debug($"Adding rci {rciId} to batch for user with id {gordonId}");

                var fullPath = System.IO.Path.Combine(this.RootFolder, RciBatchesFolder, gordonId);

                string stringContents;

                if (!this.FsDal.TryGetFileContents(fullPath, out stringContents))
                {
                    logger.Debug($"New batch! User={gordonId}, RciId{rciId}");

                    // File Does not exist, create a new one.
                    var newList = new HashSet<int> { rciId };

                    this.FsDal.WriteToFile(fullPath, JsonConvert.SerializeObject(newList));
                }
                else
                {
                    // File exists, read from it, add to it, then update it.
                    var existingList = JsonConvert.DeserializeObject<HashSet<int>>(stringContents);

                    logger.Debug($"Existing batch! User {gordonId}, Rciid={rciId}, BatchContents={string.Join(",", existingList)}");

                    existingList.Add(rciId);

                    this.FsDal.WriteToFile(fullPath, JsonConvert.SerializeObject(existingList));
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected error when trying to add rci {rciId} to batch for user {gordonId}...");

                throw;
            }
        }

        // Remove the rci from this gordonId's batch. If it doesn't exist, nothing happens.
        public void RemoveRciFromBatch(string gordonId, int rciId)
        {
            try
            {
                var fullPath = System.IO.Path.Combine(this.RootFolder, RciBatchesFolder, gordonId);

                string stringContents;

                if (!this.FsDal.TryGetFileContents(fullPath, out stringContents))
                {
                    // File does not exists, nothing to do.
                }
                else
                {
                    // File Exists, read from it, remove the id and update
                    var existingList = JsonConvert.DeserializeObject<HashSet<int>>(stringContents);

                    existingList.Remove(rciId);

                    this.FsDal.WriteToFile(fullPath, JsonConvert.SerializeObject(existingList));
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unexpected exception when trying to remove rci RciId={rciId} from {gordonId}'s batch.");

                throw;
            }
        }

        // Fetch all the rcis in the batch for this gordonId
        public HashSet<int> GetRcis(string gordonId)
        {
            try
            {
                var fullPath = System.IO.Path.Combine(this.RootFolder, RciBatchesFolder, gordonId);

                string stringContents;

                if (!this.FsDal.TryGetFileContents(fullPath, out stringContents))
                {
                    return new HashSet<int>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<HashSet<int>>(stringContents);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e, $"Unexpected exception while getting rcis in {gordonId}'s batch.");

                throw;
            }
        }
    }
}