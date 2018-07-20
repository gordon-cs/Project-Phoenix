using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.DapperDal.Sql
{
    public class CommonAreaRciSignature
    {
        public const string SimpleSelectStatement =
            @"
select sig.GordonID as GordonId,
		sig.RciId as RciId,
		sig.[Signature] as SignatureDate,
		sig.SignatureType as SignatureType
from CommonAreaRciSignature sig
";

        public const string InsertStatment =
            @"insert into CommonAreaRciSignature(GordonID, RciID, [Signature], SignatureType)
values(@GordonId, @RciId, @Signature, @SignatureType)
";

    }
}