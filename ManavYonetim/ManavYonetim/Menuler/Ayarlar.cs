using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace ManavYönetim.Menuler
{
    public partial class Ayarlar : Form
    {
        public Ayarlar()
        {
            InitializeComponent();
        }

        
        static string constring = "Data Source=DESKTOP-FDFUFH4;Initial Catalog=manav;Integrated Security=True;MultipleActiveResultSets=True";
        SqlConnection baglanti = new SqlConnection(constring);
        DataTable tablo = new DataTable();
        string User = SystemInformation.UserName;

        string dosyaKonum = Form1.dosyaKonum;

        private void Ayarlar_Load(object sender, EventArgs e)
        {
            sunucuKontrol();
            lisansKontrol();
        }

        public void sunucuKontrol()
        {
            int sonuc = 0;
            try
            {
                baglanti.Open();
                if (baglanti.State == ConnectionState.Closed)
                {
                    label6.Text = "kapalı";
                    label6.ForeColor = Color.Red;
                } else
                {
                    label6.Text = "aktif";
                    label6.ForeColor = Color.Green;
                    
                }
                baglanti.Close();

            }
            catch (Exception hata)
            {
                baglanti.Close();
                label6.Text = "kapalı";
                label6.ForeColor = Color.Red;
                MessageBox.Show("                                     Veritabanında bir sorun oluştu\n\n " + hata,"HATA");
            }

            return;
        }

        public void lisansKontrol()
        {
            int lisansVeri = lisansKontrolClass.kontrolClass();

            if (lisansVeri == 1)
            {
                label4.ForeColor = Color.Green;
                //textBox1.Text = veri;
                label4.Text = "Bulundu";
            } else
            {
                label4.ForeColor = Color.Red;
                //textBox1.Text = veri;
                label4.Text = "bulunamadı";
            }

        }

        int tabloKontrol(string isim)
        {
            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }
            int sonuc = 0;
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select case when exists((select * from INFORMATION_SCHEMA.TABLES where table_name = '" + isim + "')) then 1 else 0 end";
            komut.Connection = baglanti;
            komut.CommandType = CommandType.Text;

            SqlDataReader dr;

            dr = komut.ExecuteReader();
            while (dr.Read())
            {
                sonuc = Convert.ToInt32((dr[""]));
            }

            dr.Close();
            
            return sonuc;
        }

        private void dosyaDogrulaLabel_Click(object sender, EventArgs e)
        {
            // LİSTELER ----
            if (System.IO.Directory.Exists(dosyaKonum + "\\listeler"))
            {
                label15.ForeColor = Color.Green;

            } else
            {
                label15.ForeColor = Color.Red;
      //          MessageBox.Show(dosyaKonum + "\\listeler Konumu bulunamıyor");
            }

            // **********************

            // LİSANS ----
            if (File.Exists(dosyaKonum + "\\lisans.txt") == true)
            {
                label16.ForeColor = Color.Green;

            }
            else
            {
                label16.ForeColor = Color.Red;
       //         MessageBox.Show(dosyaKonum + "\\lisans.txt Konumu bulunamıyor");
            }

            // **********************

            // ANA DOSYA ----
            if (System.IO.Directory.Exists(dosyaKonum))
            {
                label14.ForeColor = Color.Green;

            }
            else
            {
                label14.ForeColor = Color.Red;
      //          MessageBox.Show(dosyaKonum + " Konumu bulunamıyor");
            }


            // **********************

            // MUSTERİ SECİLMEDİ ----

            if (System.IO.Directory.Exists(dosyaKonum + "\\listeler\\musteriSecilmedi"))
            {
                label17.ForeColor = Color.Green;

            }
            else
            {
                label17.ForeColor = Color.Red;
              //  MessageBox.Show(dosyaKonum + "\\listeler\\musteriSecilmedi Konumu bulunamıyor");
            }

            // **********************************************************************************************

            // VERİ TABANI *************************************


            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }

            int kontrol = tabloKontrol("kodlar");
            if(kontrol == 1)
            {
                label11.ForeColor = Color.Green;
            } else
            {
                label11.ForeColor = Color.Red;
            }

            int kontrol2 = tabloKontrol("musteriler");
            if (kontrol2 == 1)
            {
                label12.ForeColor = Color.Green;
            }
            else
            {
                label12.ForeColor = Color.Red;
            }

            int kontrol3 = tabloKontrol("kayitliTablo");
            if (kontrol3 == 1)
            {
                label9.ForeColor = Color.Green;
            }
            else
            {
                label9.ForeColor = Color.Red;
            }

            int kontrol4 = tabloKontrol("kayitliTablolar");
            if (kontrol4 == 1)
            {
                label10.ForeColor = Color.Green;
            }
            else
            {
                label10.ForeColor = Color.Red;
            }

            baglanti.Close();
            MessageBox.Show("Doya doğrulama işlemi tamamlandı");

        }

        private void listelerDoubleClick(object sender, EventArgs e)
        {
            if(label15.ForeColor == Color.Red)
            {
                Directory.CreateDirectory(dosyaKonum + "\\listeler\\");
                MessageBox.Show("Klasör Oluşturuldu");
                label15.ForeColor = Color.Green;
            }
        }

        private void lisansTxtDoubleClick(object sender, EventArgs e)
        {
            if (label16.ForeColor == Color.Red)
            {
                string dosya_yolu = @dosyaKonum + "\\lisans.txt";

                FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);

                fs.Close();
                MessageBox.Show("TXT Dosyası oluşturuldu");
                label16.ForeColor = Color.Green;
            }
        }

        private void anaDosyaDoubleClick(object sender, EventArgs e)
        {
            if (label14.ForeColor == Color.Red)
            {
                Directory.CreateDirectory(dosyaKonum);
                MessageBox.Show("Klasör Oluşturuldu");
                label14.ForeColor = Color.Green;
            }
        }

        private void musterisizListeDoubleClick(object sender, EventArgs e)
        {
            if (label17.ForeColor == Color.Red)
            {
                Directory.CreateDirectory(dosyaKonum + "\\listeler\\musteriSecilmedi");
                MessageBox.Show("Klasör Oluşturuldu");
                label17.ForeColor = Color.Green;
            }
        }

        private void kodlarTabloOlustur(object sender, EventArgs e)
        {
            if (label11.ForeColor == Color.Red)
            {
                try
                {
                    baglanti.Open();
                    string kayit = "create table kodlar(kod varChar(MAX), isim varChar(MAX))";
                    SqlCommand komut = new SqlCommand(kayit, baglanti);

                    komut.ExecuteNonQuery();

                    baglanti.Close();

                    MessageBox.Show("Tablo oluşturuldu");
                    label11.ForeColor = Color.Green;
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Bir hata oluştu " + hata);
                    baglanti.Close();
                }
            }
        }

        private void musterilerTabloOlustur(object sender, EventArgs e)
        {
            try
            {
                if (label12.ForeColor == Color.Red)
                {
                    baglanti.Open();
                    string kayit = "create table musteriler (musteri varChar(MAX), borc varChar(MAX))";
                    SqlCommand komut = new SqlCommand(kayit, baglanti);

                    komut.ExecuteNonQuery();

                    baglanti.Close();

                    MessageBox.Show("Tablo oluşturuldu");
                    label12.ForeColor = Color.Green;
                }
            }
            catch
            {
                baglanti.Close();
                MessageBox.Show("Bir hata oluştu");

            }
        }

        private void kayitliTabloOlustur(object sender, EventArgs e)
        {
            if (label9.ForeColor == Color.Red)
            {
                try
                {
                    baglanti.Open();
                    string kayit = "create table kayitliTablo (tabloIsim varChar(MAX), urunKod varChar(MAX), urunIsim varChar(MAX), parcaSayisi varChar(MAX), urunKilo varChar(MAX), urunFiyat varChar(MAX), toplamFiyat varChar(MAX))";
                    SqlCommand komut = new SqlCommand(kayit, baglanti);

                    komut.ExecuteNonQuery();

                    baglanti.Close();

                    MessageBox.Show("Tablo oluşturuldu");
                    label9.ForeColor = Color.Green;
                } catch
                {
                    MessageBox.Show("Bir hata oluştu");
                    baglanti.Close();
                }
            }
        }

        private void kayitliTablolarOlustur(object sender, EventArgs e)
        {
            if (label10.ForeColor == Color.Red)
            {
                try
                {
                    baglanti.Open();
                    string kayit = "create table kayitliTablolar (tabloIsim varChar(MAX), kayitTarih varChar(MAX))";
                    SqlCommand komut = new SqlCommand(kayit, baglanti);

                    komut.ExecuteNonQuery();

                    baglanti.Close();

                    MessageBox.Show("Tablo oluşturuldu");
                    label10.ForeColor = Color.Green;
                }catch
                {
                    MessageBox.Show("Bir hata oluştu");
                    baglanti.Close();
                }
            }
        }

        private void kurulumYap(object sender, EventArgs e)
        {
            
            dosyaDogrulaLabel_Click(sender, e);
            DialogResult bilgi = MessageBox.Show("Eksik dosyalar tespit edildi kuruluma devam etmek istiyor musunuz)", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(bilgi == DialogResult.Yes)
            {
                listelerDoubleClick(sender, e); 
                lisansTxtDoubleClick(sender, e);
                anaDosyaDoubleClick(sender, e);
                musterisizListeDoubleClick(sender, e);
                kodlarTabloOlustur(sender, e);
                musterilerTabloOlustur(sender, e);
                kayitliTabloOlustur(sender, e);
                kayitliTablolarOlustur(sender, e);
                
            } else
            {
                MessageBox.Show("İşlem iptal edildi");
            }
            
        }

        private void dosyalariTemizle(object sender, EventArgs e)
        {

            
            DialogResult bilgi = MessageBox.Show("Veri tabanı ve uygulamaya ait dosyalar silinecektir (TÜM VERİLER SİLİNİR GERİ ALINAMAZ)", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (bilgi == DialogResult.Yes)
            {


                try
                {
                    if (baglanti.State == ConnectionState.Closed)
                    {
                        baglanti.Open();
                    }

                    if (System.IO.Directory.Exists(dosyaKonum)) {
                        Directory.Delete(dosyaKonum, true);
                    }
                        

                    int kontrol = tabloKontrol("kodlar");
                    if (kontrol == 1)
                    {
                        string kayit = "DROP TABLE kodlar";
                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                        komut.ExecuteNonQuery();
                    }

                    int kontrol2 = tabloKontrol("musteriler");
                    if (kontrol == 1)
                    {
                        string kayit = "DROP TABLE musteriler";
                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                        komut.ExecuteNonQuery();
                    }

                    int kontrol3 = tabloKontrol("kayitliTablo");
                    if (kontrol3 == 1)
                    {
                        string kayit = "DROP TABLE kayitliTablo";
                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                        komut.ExecuteNonQuery();
                    }

                    int kontrol4 = tabloKontrol("kayitliTablolar");
                    if (kontrol4 == 1)
                    {
                        string kayit = "DROP TABLE kayitliTablolar";
                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                        komut.ExecuteNonQuery();
                    }

                    Directory.CreateDirectory(dosyaKonum+"\\lisans.txt");
                    MessageBox.Show("Klasör Oluşturuldu");

                    baglanti.Close();
                    dosyaDogrulaLabel_Click(sender, e);
                } catch (Exception hata)
                {
                    MessageBox.Show("Bir hata oluştu " + hata);
                    baglanti.Close();
                }
            }
            else
            {
                MessageBox.Show("İşlem iptal edildi");
            }

        }

        private void label4_DoubleClick(object sender, EventArgs e)
        {
            lisansKontrol();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Form1 gonder = new Form1();
            this.Hide();
            gonder.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            sunucuKontrol();
        }

        private void Ayarlar_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 anaSyf = new Form1();
            anaSyf.Close();
            Application.ExitThread();
            Application.Exit();
        }
    }
    
}
