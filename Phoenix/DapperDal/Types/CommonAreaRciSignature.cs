using System;

namespace Phoenix.DapperDal.Types
{
    public class CommonAreaRciSignature
    {
        public string GordonId { get; set; }
        public int RciId { get; set; }
        public DateTime? SignatureDate { get; set; }
        public string SignatureType { get; set; }
    }
}