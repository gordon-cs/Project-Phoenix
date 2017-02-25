using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Phoenix.Models;
using Phoenix.Models.ViewModels;
using System.Drawing;

namespace Phoenix.Services
{
    public class RciInputService
    {
        private RCIContext db;
        public RciInputService()
        {
            db = new Models.RCIContext();
        }

        public Rci GetRci(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci;
        }

        public string GetUsername(string gordon_id)
        {
            var username = db.Account.Where(u => u.ID_NUM == gordon_id).FirstOrDefault().AD_Username;
            return username;
        }

        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

    }
}