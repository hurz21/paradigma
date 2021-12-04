using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csTextbausteine
{
  
        using System.IO;

public partial class TB_Auswahl
    {
        public string Gruppe { get; set; } = "";
        public string subdir { get; set; } = "";
        public string inhalt { get; set; } = "";
        public string header { get; set; } = "";
        public string datei { get; set; } = "";
        public string _rootpath { get; set; }

        public TB_Auswahl(string rootpath)
        {
            _rootpath = rootpath;
        }

        public string buildSubdirPath()
        {
            string sd;
            sd = Path.Combine(_rootpath, Gruppe, subdir);
            return sd;
        }

        public string buildFullpath()
        {
            string sd;
            sd = Path.Combine(_rootpath, Gruppe, subdir);
            sd = sd + ".rtf";
            return sd;
        }
    }
}
 
