using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Borsa
{
    internal class Files
    {
    }
    public class s
    {
        public static void files()
        {
            string user = System.Environment.UserName;
            String[] directorys = {
                "borsa",
                "borsa\\dat",
                "borsa\\dat\\lastRecordDat"
            };

            String[] files =
            {
                "borsa\\options.json"
            };

            foreach(string directory in directorys)
            {
                string directoryLoc = "C:\\Users\\" + user + "\\Documents\\" + directory;
                if (Directory.Exists(directoryLoc) == false)
                {
                    Directory.CreateDirectory(directoryLoc);
                } 
            }

            foreach (string file in files)
            {
                string fileLoc = "C:\\Users\\" + user + "\\Documents\\" + file;
                if (File.Exists(fileLoc) == false)
                {
                    FileStream fs = new FileStream(fileLoc, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    string veri = String.Format(@"[
    {{
        ""ayar1"": ""secenek"",
        ""ayar2"": ""secenek""
    }}
]"
);
                    sw.WriteLine(veri);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}

        

    
    

