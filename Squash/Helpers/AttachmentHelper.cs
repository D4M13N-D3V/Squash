using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Squash.Helpers
{
    public static class AttachmentHelper
    {
        public static string GetIcon(string path)
        {
            var ext = Path.GetExtension(path);
            switch (ext)
            {
                case ".pdf":
                    return "fa fa-file-pdf-o";
                case ".mp4":
                case ".avi":
                    return "fa fa-file-video-o";
                case ".mp3":
                    return "fa fa-file-audio-o";
                case ".docx":
                    return "fa fa-file-word-o";
                case ".zip":
                case ".rar":
                case ".7zip":
                    return "fa fa-file-archive-o";
                case ".txt":
                    return "fa fa-file-text-o";
                case ".jpg":
                case ".png":
                case ".gif":
                    return "fa fa-file-image-o";
                default:
                    return "fa fa-file-o";
            }
        }
    }
}