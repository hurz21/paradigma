using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordVorlage.nsStartup
{
    class startUpHelper
    {
       public static int getArgByMarker(string marker)
        {
            int retval = 0;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string s in args)
            {

                if (s.Contains(marker))
                {//ereignissÍD
                    retval = int.Parse(s.Replace(marker, ""));
                    return retval;
                }
            }
            return 0;
        }
    }
}
