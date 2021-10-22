using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fetlife
{
    public class Project
    {
        #region constructor
        public Project()
        {
            Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CaseDownloader";
            //thread_count = 1;
        }
        #endregion

        #region Fields

        public string default_path = "%HOMEDRIVE%%HOMEPATH%/Documents/CaseDownloader";

        #endregion

        #region Properties

        public string Username { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }
        #endregion

    }
}
