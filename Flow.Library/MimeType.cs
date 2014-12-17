using System.Collections.Generic;
using System.IO;

namespace Flow.Library
{
    /// <summary>
    /// MimeType
    /// </summary>
    public static class MimeType
    {
        /// <summary>
        /// List of Mime Types
        /// </summary>
        private static Dictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            {".PDF", "application/pdf"},
            {".TXT", "text/plain"},
            {".DOC", "application/msword"},
            {".DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".XML", "text/xml"},
            {".PNG", "image/png"},
            {".GIF", "image/gif"},
            {".JPG", "image/jpeg"},
            {".JPEG", "image/jpeg"},
            {".TIFF", "image/tiff"},
            {".XSL", "application/vnd.ms-excel"},
            {".XSLX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".CONFIG", "application/xml"},
            {".CSV", "application/vnd.ms-excel"},
        };

        /// <summary>
        /// Get Mime Type
        /// </summary>
        /// <param name="fileName">FileName</param>
        /// <returns>Mime Type</returns>
        public static string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            string contentType = "application/octetstream";

            if (ext != null && MimeTypes.ContainsKey(ext.ToUpper()))
                contentType = MimeTypes[ext.ToUpper()];

            return contentType;
        }

    }
}
