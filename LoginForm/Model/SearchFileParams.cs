using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginForm.Model
{
    public class SearchFileParams
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string SortBy { get; set; }
        public string SortType { get; set; }
    }
}
