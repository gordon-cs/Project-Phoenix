using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IRciCheckoutService
    {
        FullRciViewModel GetRciById(int id);
        int AddFine(RciNewFineViewModel newFine);
        void RemoveFine(int fineID);
        void CheckoutCommonAreaMemberSignRci(int rciID, string gordonID);
        void CheckoutRASignRci(int rciID, string raGordonID);
        bool CheckoutRDSignRci(int rciID, string rdGordonID);
        void CheckoutResidentSignRci(int rciID);
        HttpStatusCode PostWorkRequest(HttpClient client, string workRequest, string ADUsername, string fullBuildingName, string roomNumber, string phoneNumber, string firstname, string lastname, string gordonID);
        void SendFineEmail(int rciID, string emailAddress, string password);
        void WorkRequestDamages(List<string> workRequests, string username, string password, int rciID, string phoneNumber);
    }
}