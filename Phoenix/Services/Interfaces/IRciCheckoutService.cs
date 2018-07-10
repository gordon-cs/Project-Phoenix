using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IRciCheckoutService
    {
        int AddFine(RciNewFineViewModel newFine);
        void CheckoutCommonAreaMemberSignRci(int rciID, string gordonID);
        void CheckoutRASignRci(int rciID, string raGordonID);
        bool CheckoutRDSignRci(int rciID, string rdGordonID);
        void CheckoutResidentSignRci(int rciID);
        Rci GetBareRciByID(int id);
        CheckoutCommonAreaRciViewModel GetCommonAreaRciByID(int id);
        IEnumerable<string> GetCommonRooms(int id);
        GenericCheckoutViewModel GetGenericCheckoutRciByID(int id);
        CheckoutIndividualRoomRciViewModel GetIndividualRoomRciByID(int id);
        HttpStatusCode PostWorkRequest(HttpClient client, string workRequest, string ADUsername, string fullBuildingName, string roomNumber, string phoneNumber, string firstname, string lastname, string gordonID);
        void RemoveFine(int fineID);
        void SendFineEmail(int rciID, string emailAddress, string password);
        void WorkRequestDamages(List<string> workRequests, string username, string password, int rciID, string phoneNumber);
    }
}