using System.Collections.Generic;

namespace Phoenix.Services
{
    public interface IRciBatchService
    {
        void AddRciToBatch(string gordonId, int rciId);
        void RemoveRciFromBatch(string gordonId, int rciId);
        HashSet<int> GetRcis(string gordonId);
    }
}