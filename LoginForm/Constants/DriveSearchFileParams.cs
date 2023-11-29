using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginForm.Constants
{
    public class DriveSearchFileParams
    {
        public const string Name = "name =";
        public const string NameContains = "name contains ";
        public const string IsFolder = "mimeType = 'application/vnd.google-apps.folder'";
        public const string IsNotFolder = "mimeType != 'application/vnd.google-apps.folder'";
        public const string Root = "'root' in parents and trashed = false";
    }
}
