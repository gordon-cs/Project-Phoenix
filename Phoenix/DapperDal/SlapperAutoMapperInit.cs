using Phoenix.DapperDal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.DapperDal
{
    public static class SlapperAutoMapperInit
    {
        public static void Initialize()
        {
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Rci), new List<string> { "RciId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(SmolRci), new List<string> { "RciId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(BigRci), new List<string> { "RciId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(CommonAreaRciSignature), new List<string> { "CommonAreaSignatureGordonId", "CommonAreaSignatureRciId", "CommonAreaSignatureType" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(RoomComponentType), new List<string> { "RoomComponentTypeId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Fine), new List<string> { "FineId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Damage), new List<string> { "DamageId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(Account), new List<string> { "GordonId" });
            Slapper.AutoMapper.Configuration.AddIdentifiers(typeof(ResidentHallGrouping), new List<string> { "HallGroup" });
        }
    }
}