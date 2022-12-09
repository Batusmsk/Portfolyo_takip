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
            Form1 form = new Form1();

            string user = System.Environment.UserName;
            String[] directorys = {
                "borsa",
                "borsa\\dat",
                "borsa\\logs",
                "borsa\\dat\\lastRecordDat"     
            };

            String[] files =
            {
                "borsa\\options.json",
                "borsa\\userOptions.json"
            };

            foreach(string directory in directorys)
            {
                string directoryLoc = "C:\\Users\\" + user + "\\Documents\\" + directory;
                if (Directory.Exists(directoryLoc) == false)
                {
                    Directory.CreateDirectory(directoryLoc);
                    
                    form.WriteLog("Eksik klasör oluşturuldu " + directoryLoc);
                } 
            }

            foreach (string file in files)
            {
                string fileLoc = "C:\\Users\\" + user + "\\Documents\\" + file;
                if (File.Exists(fileLoc) == false)
                {
                    FileStream fs = new FileStream(fileLoc, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    string veri = "";
                    if (file == "borsa\\options.json")
                    {
                        
                         veri = String.Format(@"[{{""ayar1"": ""0"",""ayar2"": ""secenek""}}]");


                    } else if (file.Contains("userOptions.json"))
                    {
                        veri = (@"[{""balance"": ""0""}]");
                    }
                    form.WriteLog("Eksik dosya oluşturuldu " + fileLoc);
                    sw.WriteLine(veri);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}

        

    
    

