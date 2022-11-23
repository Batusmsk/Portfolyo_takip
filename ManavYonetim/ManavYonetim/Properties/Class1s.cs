using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace WindowsFormsApp2
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
            int sonuc = 0;

            string dosya_yolu = @dosyaKonum + "\\lisans.txt";
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
                sonuc = 1;
            }
            else
            {
                sonuc = 0;
            }
            sw.Close();
            fs.Close();
            return sonuc;
        }
    }

}
