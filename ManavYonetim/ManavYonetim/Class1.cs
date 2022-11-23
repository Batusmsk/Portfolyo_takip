using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace ManavYönetim
{
    internal class Class1
    {
    }

    

    public class lisansKontrolClass
    {

        static public int kontrolClass()
        {
            string dosyaKonum = Form1.dosyaKonum;
            string lisans = "Qmöd0jfN94-S3nJsEÖk8skNjsf";
            string User = SystemInformation.ComputerName;
            int sonuc = 0;

            string dosya_yolu = @dosyaKonum + "\\lisans.txt";

            

            if (File.Exists(dosya_yolu) == true)
            {
                FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
                StreamReader sw = new StreamReader(fs);
                string veri = "";
                string yazi = sw.ReadLine();
                while (yazi != null)
                {

                    //MessageBox.Show(yazi.Substring(1));
                    veri = yazi;

                    yazi = sw.ReadLine();
                }


                if (veri == lisans)
                {
                    if (User == "DESKTOP-FDFUFH4")
                    {
                        sonuc = 1;
                    }
                    else
                    {
                        sonuc = 0;
                    }
                }
                else
                {
                    sonuc = 0;
                }
                sw.Close();
                fs.Close();
                
            } else
            {
                MessageBox.Show("Lisans.txt dosyası bulunamadı");
                sonuc = 0;
            }
            return sonuc;

        }
    }

}
