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
using System.Reflection.Emit;
using Label = System.Windows.Forms.Label;

namespace ManavYönetim.Menuler
{
    public partial class Musteriler : Form
    {
        public Musteriler()
        {
            InitializeComponent();
        }
        
        string dosyaKonum = Form1.dosyaKonum;
        string User = SystemInformation.UserName;
        static string constring = "Data Source=DESKTOP-FDFUFH4;Initial Catalog=manav;Integrated Security=True;MultipleActiveResultSets=True;MultipleActiveResultSets=True";
        SqlConnection baglanti = new SqlConnection(constring);
        DataTable tablo = new DataTable();



        private void dosyayaYaz(string isim)
        {
            string tarih = DateTime.Now.ToString("dd.MM.yyyy");
            string saat = DateTime.Now.ToString("H:mm:ss");

            string dosya_yolu = @dosyaKonum + "\\listeler\\" + isim + "\\bilgiler.txt";

            FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(tarih + " " + saat);

            sw.Flush();

            sw.Close();
            fs.Close();
        }

        public void dosyadanOku(string isim)
        {
            
            string dosya_yolu = @dosyaKonum+"\\listeler\\" + isim + "\\bilgiler.txt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);

            StreamReader sw = new StreamReader(fs);
            
            string yazi = sw.ReadLine();
            while (yazi != null)
            {

                //MessageBox.Show(yazi.Substring(1));
                string veri = yazi;
                string saat = veri.Split(' ').Last();
                string tarih = veri.Split(' ').First();

                textBox2.Text = tarih;
                textBox3.Text = saat;
                yazi = sw.ReadLine();
            }

            sw.Close();
            fs.Close();
 
        }

        private void musteriListele()
        {
            listBox1.Items.Clear();
           
            string[] klasorler = Directory.GetDirectories(dosyaKonum + "\\listeler\\");
            
            for (int j = 0; j < klasorler.Length; j++)
            {
                
                string dosya = klasorler[j];
                string dosyaIsim = dosya.Split('\\').Last();

                if (dosyaIsim != "musteriSecilmedi")
                {
                    listBox1.Items.Add(dosyaIsim);
                }
            }
        }

        private void musteriComboBoxYenile()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Text = "";
            comboBox2.Text = "";
            string[] klasorler = Directory.GetDirectories(dosyaKonum+"\\listeler");
           
            for (int j = 0; j < klasorler.Length; j++)
            {
                
                string dosya = klasorler[j];
                string dosyaIsim = dosya.Split('\\').Last();
                if (dosyaIsim != "musteriSecilmedi")
                {
                    comboBox1.Items.Add(dosyaIsim);
                    comboBox2.Items.Add(dosyaIsim);
                }

            }

        }

        private void musteriOlusturBtn_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength < 1)
            {
                MessageBox.Show("Lütfen müşteri isimi yazınız");
                return;
            }

            if (System.IO.Directory.Exists(dosyaKonum+"\\listeler\\" + textBox1.Text))
            {
                MessageBox.Show("Böyle bir müşteri bulunuyor");

            }
            else
            {
                
                if(baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }
                string kayit = "insert into musteriler (musteri, borc) values(@musteri,0)";

                SqlCommand komut = new SqlCommand(kayit, baglanti);
                komut.Parameters.AddWithValue("@musteri", textBox1.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();


                Directory.CreateDirectory(dosyaKonum+"\\listeler\\" + textBox1.Text);
                dosyayaYaz(textBox1.Text);
                musteriListele();
                musteriComboBoxYenile();
                MessageBox.Show("Müşteri oluşturuldu");
            }


        }

        private void Musteriler_Load(object sender, EventArgs e)
        {
            
            //MessageBox.Show(dosya);
            musteriListele();
            musteriComboBoxYenile();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listBox1.SelectedItem != null)
            {
                textBox12.Text = listBox1.SelectedItem.ToString();
                comboBox1.Text = listBox1.SelectedItem.ToString();
                dosyadanOku(listBox1.SelectedItem.ToString());
            }



        }

        private void musteriSil_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen seçim yapın");
                return;
            }
            if (System.IO.Directory.Exists(dosyaKonum+"\\listeler\\" + comboBox1.Text))
            {
                DialogResult onay = MessageBox.Show("Müşteriyi ile birlikte borç ve liste bilgileri silinecektir", "UYARI", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
               
                if(onay == DialogResult.Cancel)
                {
                    MessageBox.Show("Müşteri silinmedi");
                    return;
                }

                Directory.Delete(dosyaKonum+"\\listeler\\" + comboBox1.Text, true);

                textBox2.Text = "";
                textBox3.Text = "";
                musteriListele();
                musteriComboBoxYenile();



                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();

                }
                SqlCommand komut = new SqlCommand("delete from musteriler where musteri=@kod", baglanti);
                komut.Parameters.AddWithValue("@kod", textBox12.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Müşteri silindi");



            }
            else
            {
                MessageBox.Show("Böyle bir müşteri bulunmuyor");

            }
        }

        private void musteriKayit_Click(object sender, EventArgs e)
        {
            if (textBox4.TextLength >= 1)
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Seçili bir müşteri bulunmuyor");
                    return;
                }

                if (System.IO.Directory.Exists(dosyaKonum + textBox4.Text))
                {
                    MessageBox.Show("Böyle bir müşteri bulunuyor");
                    return;
                }
                Directory.Move(dosyaKonum+"\\listeler\\" + comboBox1.Text, dosyaKonum+"\\listeler\\" + textBox4.Text);

                textBox2.Text = "";
                textBox3.Text = "";
                musteriListele();
                musteriComboBoxYenile();

                if(baglanti.State == ConnectionState.Closed) {
                    baglanti.Open();
                } 
                string kayit = "UPDATE musteriler SET musteri=@isim WHERE musteri='" + textBox12.Text + "'";
                

                SqlCommand komut = new SqlCommand(kayit, baglanti);
                komut.Parameters.AddWithValue("@isim", textBox4.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();
                MessageBox.Show("Müşteri isimi düzenlendi");

            }
            else
            {
                MessageBox.Show("Lütfen değiştirilecek isim giriniz");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox12.Text = comboBox1.Text;
            dosyadanOku(comboBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            textBox9.Enabled = true;
            textBox10.Enabled = true;
            textBox11.Enabled = true;

            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox6.Text = comboBox2.Text;
            listBox2.Items.Clear();

            string[] dosyalar = System.IO.Directory.GetFiles(dosyaKonum + "\\listeler\\" + comboBox2.Text);

            for (int j = 0; j < dosyalar.Length; j++)
            {
                string dosya = dosyalar[j];
                string dosyaIsim = dosya.Split('\\').Last();
                if (dosyaIsim != "bilgiler.txt")
                {
                    string dosyaIsimFix = dosyaIsim;
                    listBox2.Items.Add(dosyaIsimFix.Replace(".pdf", ""));
                }

            }

            baglanti.Open();

            SqlCommand cmd2 = new SqlCommand("select * from musteriler where musteri=@veri2", baglanti);
            cmd2.Parameters.AddWithValue("@veri2", comboBox2.Text);
            SqlDataReader dr = cmd2.ExecuteReader();
            if (dr.Read())
            {
                textBox9.Text = dr["borc"].ToString();
            } else
            {
                textBox9.Text = "0";
            }
            dr.Close();
            baglanti.Close();
            
            //MessageBox.Show(vScrollBar1.Maximum.ToString());

        }

        private void musterisizListeGetir_Click(object sender, EventArgs e)
        {
            string[] dosyalar;
            try
            {
                listBox2.Items.Clear();
                textBox6.Text = "musteriSecilmedi";
                dosyalar = System.IO.Directory.GetFiles(dosyaKonum + "\\listeler\\musteriSecilmedi");
                comboBox2.Text = "";

                textBox9.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";

                textBox9.Enabled = false;
                textBox10.Enabled = false;
                textBox11.Enabled = false;

                //MessageBox.Show(Convert.ToString(dosyalar.Length));
            }
            catch
            {
                MessageBox.Show("Bir hata oluştu");
                return;
            }



            //MessageBox.Show(dosyalar[16]);


            for (int j = dosyalar.Length; j >= 0; j--)
            {
               // MessageBox.Show(j.ToString());
                try
                {
                    string dosya = dosyalar[j - 1];
                    string dosyaIsim = dosya.Split('\\').Last();
                    if (dosyaIsim != "bilgiler.txt")
                    {
                        string dosyaIsimFix = dosyaIsim;
                        listBox2.Items.Add(dosyaIsimFix.Replace(".pdf", ""));

                    }
                }
                catch { }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                char[] ayrac = { ' ', '-' };

                string listeAd = listBox2.SelectedItem.ToString();
                listeIsim_Text.Text = listeAd;
                string[] veri = listeAd.Split(ayrac);

                

                string tarih = veri[0];
                string saat = veri[1].Replace(";", ":");
                string isim = listeAd.Split('-').Last().Replace(".pdf", "");
                
                string isimFix = isim.Substring(1, isim.Length - 1);
             
                textBox5.Text = isimFix;
                textBox7.Text = tarih;
                textBox8.Text = saat;
                
            }
        }

        private void borcKayitEt_Click(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen müşteri seçimi yapınız");
                return;
            }
             


            int borcMiktari = 0;
            int yeniBorc = 0;

            if(baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }
            if(textBox9.TextLength < 1 && textBox10.TextLength < 1 && textBox11.TextLength < 1)
            {
                MessageBox.Show("Borç düzenlerken miktar belirtmelisiniz");
                return;
                baglanti.Close();
            }

            try
            {
                SqlCommand cmd2 = new SqlCommand("select * from musteriler where musteri=@veri2", baglanti);
                cmd2.Parameters.AddWithValue("@veri2", comboBox2.Text);
                SqlDataReader dr = cmd2.ExecuteReader();
                if (dr.Read())
                {
                    borcMiktari = Convert.ToInt32(dr["borc"]);
                }
                dr.Close();


                if (Convert.ToInt32(textBox9.Text) != borcMiktari)
                {
   
                    string kayit = "UPDATE musteriler SET borc=@borc WHERE musteri='" + comboBox2.Text + "'";
                    yeniBorc = Convert.ToInt32(textBox9.Text);
                    SqlCommand komut = new SqlCommand(kayit, baglanti);
                    komut.Parameters.AddWithValue("@borc", yeniBorc);
                    komut.ExecuteNonQuery();
                    


                }

                if (textBox11.TextLength >= 1)
                {

                    string kayit = "UPDATE musteriler SET borc=@borc WHERE musteri='" + comboBox2.Text + "'";
                    yeniBorc = Convert.ToInt32(textBox11.Text) + borcMiktari;

                    SqlCommand komut = new SqlCommand(kayit, baglanti);
                    komut.Parameters.AddWithValue("@borc", yeniBorc);
                    komut.ExecuteNonQuery();
                    



                }

                if (textBox10.TextLength >= 1)
                {

                    if (Convert.ToInt32(textBox10.Text) <= borcMiktari)
                    {
                        string kayit = "UPDATE musteriler SET borc=@borc WHERE musteri='" + comboBox2.Text + "'";
                        yeniBorc = borcMiktari - Convert.ToInt32(textBox10.Text);

                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                        komut.Parameters.AddWithValue("@borc", yeniBorc);
                        komut.ExecuteNonQuery(); 


                    }
                    else
                    {
                        MessageBox.Show("Çıkarılacak miktar borç miktarından yüksek olamaz", "Hata");
                        baglanti.Close();
                        return;
                    }

                }

                textBox9.Text = yeniBorc.ToString();
                MessageBox.Show("Borç miktarı düzenlendi");
                baglanti.Close();
            }
            catch(Exception hata)
            {
                MessageBox.Show("Bir hata oluştu Hata: " + hata.Message);
                baglanti.Close();
            }

        }

        private void listeSil_Click(object sender, EventArgs e)
        {
            if(textBox6.TextLength >= 1)
            {
                if(listeIsim_Text.TextLength >= 1)
                {

                    string konum = textBox6.Text;
                    string isim = listeIsim_Text.Text;
                    string dosya_dizini = (dosyaKonum + "\\listeler\\" + konum + "\\"+isim+".pdf");

                    if (File.Exists(dosya_dizini) == true)
                    {

                        DialogResult secenek = MessageBox.Show("Seçili listeyi silmek istediğinize emin misiniz?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (secenek == DialogResult.Yes)
                        {
                            System.IO.File.Delete(dosya_dizini);
                            listBox2.Items.Remove(isim);
                            MessageBox.Show("Seçili liste silindi");
                        }
                        else if (secenek == DialogResult.No)
                        {
                            MessageBox.Show("İşlem iptal edildi");
                        }


                    }
                    else
                    {
                        MessageBox.Show("Bir hata oluştu böyle bir liste bulunamıyor", "Hata");
                    }

                } else
                {
                    MessageBox.Show("Öncelikle silmek istediğiniz listeyi seçin", "Hata");
                }

            } else
            {
                MessageBox.Show("Bir hata oluştu dosya konumu bulunamıyor", "HATA");
            }
        }

        private void listeAc_Click(object sender, EventArgs e)
        {
            if (textBox6.TextLength >= 1)
            {
                if (listeIsim_Text.TextLength >= 1)
                {

                    string konum = textBox6.Text;
                    string isim = listeIsim_Text.Text;
                    string dosya_dizini = (dosyaKonum + "\\listeler\\" + konum + "\\" + isim + ".pdf");

                    if (File.Exists(dosya_dizini) == true)
                    {

                        System.Diagnostics.Process.Start(dosya_dizini);


                    }
                    else
                    {
                        MessageBox.Show("Bir hata oluştu böyle bir liste bulunamıyor", "Hata");
                    }

                }
                else
                {
                    MessageBox.Show("Öncelikle açmak istediğiniz listeyi seçin", "Hata");
                }

            }
            else
            {
                MessageBox.Show("Bir hata oluştu dosya konumu bulunamıyor", "HATA");
            }
            
        }

        private void listeleriSil_Click(object sender, EventArgs e)
        {
            if(textBox6.TextLength >= 1)
            {
                string konum = textBox6.Text;
  
                string dosya_dizini = (dosyaKonum + "\\listeler\\" + konum);

                if (System.IO.Directory.Exists(dosyaKonum + "\\listeler\\" + konum))
                {
                    string[] folder = Directory.GetFiles(@dosyaKonum + "\\listeler\\" + konum + "\\");
                    DialogResult secenek = MessageBox.Show("Müşteriye ait tüm listeleri silmek istediğinize emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (secenek == DialogResult.Yes)
                    {
                        foreach (string _file in folder)
                        {
                            if(_file.Split('\\').Last() != "bilgiler.txt")
                            {
                                File.Delete(_file);
                               // MessageBox.Show(_file.Split('\\').Last());
                            }
                            
                            
                            

                        }
                        MessageBox.Show("Listeler Silindi");
                        listBox2.Items.Clear();
                    }
                    else if (secenek == DialogResult.No)
                    {
                        MessageBox.Show("İşlem iptal edildi");
                    }

                } else
                {
                    MessageBox.Show("Böyle bir müşteri bulunamıyor");
                }
            } else
            {
                MessageBox.Show("Öncelikle müşteri seçmelisiniz");
            }

        }

        private void textBox6_DoubleClick(object sender, EventArgs e)
        {
            if (textBox6.TextLength >= 1)
            {

                    string konum = textBox6.Text;
                    string isim = listeIsim_Text.Text;
                    string dosya_dizini = (dosyaKonum + "\\listeler\\" + konum);



                        System.Diagnostics.Process.Start(dosyaKonum+"\\listeler\\"+konum);






            }
            else
            {
                MessageBox.Show("Bir hata oluştu dosya konumu bulunamıyor", "HATA");
            }
        }

        private void txt_sadece_sayi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void anaMenuGonder(object sender, EventArgs e)
        {
            Form1 gonder = new Form1();
            this.Hide();
            gonder.Show();
        }

        private void kodlarSayfasiGonder(object sender, EventArgs e)
        {
            kodlar gonder = new kodlar();
            this.Hide();
            gonder.Show();
        }

        private void listelerSayfasiGonder(object sender, EventArgs e)
        {
            liste gonder = new liste();
            this.Hide();
            gonder.Show();
        }

        private void ayarlarSayfasiGonder(object sender, EventArgs e)
        {
            Menuler.Ayarlar gonder = new Menuler.Ayarlar();
            this.Hide();
            gonder.Show();
        }

        private void borcEkleChanged(object sender, EventArgs e)
        {
            if(baglanti.State == ConnectionState.Closed) {
                baglanti.Open();
            }

            int borcMiktari = 0;
            SqlCommand cmd2 = new SqlCommand("select * from musteriler where musteri=@veri2", baglanti);
            cmd2.Parameters.AddWithValue("@veri2", comboBox2.Text);
            SqlDataReader dr = cmd2.ExecuteReader();
            if (dr.Read())
            {
                borcMiktari = Convert.ToInt32(dr["borc"]);
            }
            dr.Close();
            baglanti.Close();

            if (textBox11.TextLength >= 1)
            {
                textBox9.Text = Convert.ToString(borcMiktari);
                textBox9.Enabled = false;
                textBox10.Text = "";
                textBox10.Enabled = false;
            } else {
                textBox9.Enabled = true;
                textBox10.Enabled = true;
            }

        }

        private void borcCikarChanged(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }

            int borcMiktari = 0;
            SqlCommand cmd2 = new SqlCommand("select * from musteriler where musteri=@veri2", baglanti);
            cmd2.Parameters.AddWithValue("@veri2", comboBox2.Text);
            SqlDataReader dr = cmd2.ExecuteReader();
            if (dr.Read())
            {
                borcMiktari = Convert.ToInt32(dr["borc"]);
            }
            dr.Close();
            baglanti.Close();

            if (textBox10.TextLength >= 1)
            {
                textBox9.Text = Convert.ToString(borcMiktari);
                textBox9.Enabled = false;
                textBox11.Text = "";
                textBox11.Enabled = false;
            }
            else
            {
                textBox9.Enabled = true;
                textBox11.Enabled = true;
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                try
                {
                    string konum = textBox6.Text;
                    string isim = listeIsim_Text.Text;
                    string dosya_dizini = (dosyaKonum + "listeler\\" + konum);
                //    MessageBox.Show(dosyaKonum + "listeler\\" + konum + "\\" + listBox2.SelectedItem.ToString());
                    System.Diagnostics.Process.Start(dosyaKonum + "\\listeler\\" + konum + "\\" + listBox2.SelectedItem.ToString()+".pdf");
                } catch
                {
                    MessageBox.Show("Liste açılırken bir hata oluştu", "HATA");
                }
            }

             
        }

        private void Musteriler_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 anaSyf = new Form1();
            anaSyf.Close();
            Application.ExitThread();
            Application.Exit();
        }

        private void list2_Paint(object sender, PaintEventArgs e)
        {

        }



        private void label21_Click(object sender, EventArgs e)
        {

        }
    }
}


