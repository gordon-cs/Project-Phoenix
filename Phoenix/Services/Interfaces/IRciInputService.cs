using System.Collections.Generic;
using System.Drawing;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IRciInputService
    {
        void ApplyExifData(Image image);
        void CheckRcis(int sigCheck, string gordonID, int id);
        CheckinCommonAreaRciViewModel GetCommonAreaRciById(int id);
        IEnumerable<string> GetCommonRooms(int id);
        string GetName(string gordonID);
        Rci GetRci(int id);
        IEnumerable<SignAllRDViewModel> GetRcisForBuildingThatCanBeSignedByRD(List<string> buildingCode);
        string GetUsername(string gordon_id);
        Size NewImageSize(Size imageSize, Size newSize);
        void ResizeImage(Image origImg, Image newImage, Size imgSize);
        bool SaveCommonAreaMemberSig(string rciSig, string user, string gordonID, int rciID);
        void SaveImagePath(string fullPath, Damage damage);
        void SavePhotoDamage(Damage damage, string rciComponentId);
        bool SaveRASigs(string rciSig, string lacSig, string rciSigRes, string user, int id, string gordonID);
        bool SaveRDSigs(string rciSig, string user, int id, string gordonID);
        bool SaveResSigs(string rciSig, string lacSig, string user, int id);
        void SignRcis(string gordonID);
    }
}