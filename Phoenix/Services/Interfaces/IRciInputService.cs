using System.Collections.Generic;
using System.Drawing;
using Phoenix.Models;
using Phoenix.Models.ViewModels;

namespace Phoenix.Services
{
    public interface IRciInputService
    {
        void CheckRcis(int sigCheck, string gordonID, int rciId);
        FullRciViewModel GetRci(int id);
        IEnumerable<SignAllRDViewModel> GetRcisForBuildingThatCanBeSignedByRD(string gordonId, List<string> buildingCodes);
        Size NewImageSize(Size imageSize, Size newSize);
        void ApplyExifData(Image image);
        void ResizeImage(Image origImg, Image newImage, Size imgSize);
        string FetchDamageFilePath(int damageId);
        int SaveTextDamage(string description, int rciId, string gordonId, int roomComponentTypeId);
        int SavePhotoDamage(string imagePath, int rciId, string gordonId, int roomComponentTypeId);
        void DeleteDamage(int damageId);
        bool SaveCommonAreaMemberSig(string rciSig, string user, string gordonID, int rciID);
        bool SaveRASigs(string rciSig, string lacSig, string rciSigRes, string user, int id, string gordonID);
        bool SaveRDSigs(string rciSig, string user, int id, string gordonID);
        bool SaveResSigs(string rciSig, string lacSig, string user, int id);
        void SignRcis(string gordonID);
    }
}