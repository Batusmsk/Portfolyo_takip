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
    public partial class Form1 : Form
    {
        static string User = SystemInformation.UserName;
        public static string dosyaKonum = "C:\\Users\\"+User+"\\Documents\\ManavYönetim\\";
        public Form1()
        {
            InitializeComponent();
        }
        //static string constring = "Data Source=DESKTOP-FDFUFH4;Initial Catalog=manav;Integrated Security=True";
        //SqlConnection connect = new SqlConnection(constring);
        bool lisansDurum = false;

       
        private void lisansKontrol()
        {
            int lisansVeri = lisansKontrolClass.kontrolClass();

            if (lisansVeri == 1)
            {
                lisansDurum = true;
            }
            else
            {
                lisansDurum = false;
                MessageBox.Show("Lisans hatası");
            }


        }



        private void Form1_Load(object sender, EventArgs e)
        {
            //this.DesktopLocation = new Point(600,200);
            //this.MinimumSize = new System.Drawing.Size(this.Width, Screen.PrimaryScreen.Bounds.Height);
            lisansKontrol();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lisansKontrol();
        }

        private void kodSayfasiGonder(object sender, EventArgs e)
        {
            if (!lisansDurum)
            {

            }
            else
            {
                kodlar kod = new kodlar();
                this.Hide();
                kod.Show();

            }

        }

        private void listeSayfasiGonder(object sender, EventArgs e)
        {
            if (!lisansDurum)
            {

            }
            else
            {

                liste list = new liste();
                this.Hide();
                list.Show();
            }
        }

        private void musterilerSayfasiGonder(object sender, EventArgs e)
        {
            if (!lisansDurum)
            {

            }
            else
            {

                Menuler.Musteriler musteriler = new Menuler.Musteriler();
                this.Hide();
                musteriler.Show();
            }
        }

        private void ayarlarSayfasiGonder(object sender, EventArgs e)
        {

                Menuler.Ayarlar ayarlar = new Menuler.Ayarlar();
                this.Hide();
                ayarlar.Show();
            

        }


    }
}

