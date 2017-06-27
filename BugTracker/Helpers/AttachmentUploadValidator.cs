using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{

        public static class AttachmentUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase file)
            {
                if (file == null)
                    return false;

                if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;

            //try

            //{
            //    using (var img = Image.FromStream(file.InputStream))
            //    {
            //        return ImageFormat.Jpeg.Equals(img.RawFormat) ||
            //               ImageFormat.Png.Equals(img.RawFormat) ||
            //               ImageFormat.Gif.Equals(img.RawFormat);
            //    }
            //}

            //catch

            //{
            //    return false;
            //}

            return true;

            }
        }

    
}