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

        // Image resize method, taken mostly from  http://www.advancesharp.com/blog/1130/image-gallery-in-asp-net-mvc-with-multiple-file-and-size
        // Takes an original size, and returns proportional dimensions to be used in resizing the image
        public Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            //double tempval;
            int origHeight = imageSize.Height;
            int origWidth = imageSize.Width;


            if (origHeight > newSize.Height || origWidth > newSize.Width)
            {
                // Calculate the resizing ratio based on whichever is bigger - original height or original width
                decimal tempVal = origHeight > origWidth ? decimal.Divide(newSize.Height, origHeight) : decimal.Divide(newSize.Width, origWidth);
                //if (imageSize.Height > imageSize.Width)
                //    tempval = newSize.Height / (imageSize.Height * 1.0);
                //else
                //    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempVal * imageSize.Width), (int)(tempVal * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        public IEnumerable<string> GetCommonRooms(int id)
        {
            var rci = db.Rci.Where(m => m.RciID == id).FirstOrDefault();
            return rci.RciComponent.GroupBy(x => x.RciComponentDescription).Select(x => x.First()).Select(x => x.RciComponentDescription);
        }

    }
}