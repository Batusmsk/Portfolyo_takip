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

namespace ManavYönetim
{
    public partial class kodlar : Form
    {
        public kodlar()
        {
            InitializeComponent();
        }

        static string constring = "Data Source=DESKTOP-FDFUFH4;Initial Catalog=manav;Integrated Security=True";
        SqlConnection baglanti = new SqlConnection(constring);
        DataTable tablo = new DataTable();
        int menuKontrol;


        private void kayitGetir()
        {
            baglanti.Open();
            string kayit = "SELECT * from kodlar";
            //musteriler tablosundaki tüm kayıtları çekecek olan sql sorgusu.
            SqlCommand komut = new SqlCommand(kayit, baglanti);
            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            SqlDataAdapter da = new SqlDataAdapter(komut);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridView1.DataSource = dt;
            //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
            baglanti.Close();
        }

        private void kodlar_Load(object sender, EventArgs e)
        {
            kayitGetir();


            textBox3.Enabled = false;
            bilgiMesaji("Bilgi", "Ana sayfa'ya geri dönmek için tıklayın.", pictureBox1);
            bilgiMesaji("Bilgi", "Liste oluşturucu sayfasına gitmek için tıklayın.", pictureBox3);
            bilgiMesaji("Bilgi", "Müşteriler sayfasına gitmek için tıklayın.", pictureBox4);
            bilgiMesaji("Bilgi", "Ayarlar sayfasına gitmek için tıklayın.", pictureBox2);
            bilgiMesaji("Bilgi", "Düzenleme yaparken kod değiştirelemez.", label3);


           // panel4.Location = new Point(55, 325);
            //panel2.Location = new Point(55, 107);



        }

        ToolTip bilgiMesaji(string baslik, string aciklama, Control nesne)
        {
            ToolTip bilgi = new ToolTip();
            bilgi.Active = true;
            bilgi.ToolTipTitle = baslik;
            bilgi.ToolTipIcon = ToolTipIcon.Info;
            bilgi.UseFading = true; // silik olarak kaybolup yükleme
            bilgi.UseAnimation = true;
            bilgi.IsBalloon = true;
            bilgi.ShowAlways = true;
            bilgi.AutoPopDelay = 2500; // mesajın açık kalma süresi
            bilgi.ReshowDelay = 2000; // mause çektikten sonra kaç sn açık lasın
            bilgi.InitialDelay = 800; // açılma süre
            bilgi.BackColor = Color.White;
            bilgi.ForeColor = Color.DarkBlue;
            bilgi.SetToolTip(nesne, aciklama);

            return bilgi;
        }

        private void kodEkleButton(object sender, EventArgs e)
        {
            try
            {

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();

                }

                if (textBox1.TextLength >= 1 && textBox2.TextLength >= 1)
                {

                    string aranan = Convert.ToString(textBox1.Text);
                    for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                            {
                                if (cell.Value != null)
                                {
                                    if (cell.Value.ToString().ToUpper() == aranan)
                                    {

                                        MessageBox.Show("Zaten bu numarada bir ürün bulunuyor.");
                                        return;
                                    }
                                }
                                else
                                {
                                    string kayit = "insert into kodlar (kod, isim) values(@kod,@isim)";
                                    SqlCommand komut = new SqlCommand(kayit, baglanti);

                                    komut.Parameters.AddWithValue("@kod", textBox1.Text);

                                    komut.Parameters.AddWithValue("@isim", textBox2.Text);


                                    komut.ExecuteNonQuery();

                                    baglanti.Close();
                                    MessageBox.Show("Eklendi");
                                    kayitGetir();
                                    return;
                                }
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Ürün numarası ve ürün isimini giriniz.");
                }

            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata meydana geldi" + hata.Message);
            }
        }

        private void kodSilButton(object sender, EventArgs e)
        {
            if (textBox3.TextLength >= 1)
            {

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();

                }
                SqlCommand komut = new SqlCommand("delete from kodlar where kod=@kod", baglanti);
                komut.Parameters.AddWithValue("@kod", textBox3.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();
                kayitGetir();

                textBox3.Text = "";
                textBox4.Text = "";


            }
            else
            {
                MessageBox.Show("Silmek istediğiniz ürünü tablodan seçiniz veya ürün numarasını yazınız.");
            }


        }

        private void kodKayitEtButton(object sender, EventArgs e)
        {
            try
            {

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();

                }

                if (textBox3.TextLength >= 1 && textBox4.TextLength >= 1)
                {

                    string aranan = Convert.ToString(textBox3.Text);
                    for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
                            {
                                if (cell.Value != null)
                                {
                                    if (cell.Value.ToString().ToUpper() == aranan)
                                    {

                                        string kayit = "UPDATE kodlar SET isim=@isim WHERE kod=@kod";
                                        SqlCommand komut = new SqlCommand(kayit, baglanti);
                                        komut.Parameters.AddWithValue("@kod", textBox3.Text);
                                        komut.Parameters.AddWithValue("@isim", textBox4.Text);
                                        komut.ExecuteNonQuery();

                                        baglanti.Close();
                                        MessageBox.Show("Değişim Kayıt Edildi", "Bilgi");
                                        //MessageBox.Show(dataGridView1.CurrentRow.Cells[1].Value.ToString().ToUpper() + textBox4.Text.ToUpper());
                                        kayitGetir();


                                        return;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Böyle bir ürün bulunmuyor.");
                                    return;
                                }
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Ürün numarası ve ürün isimini giriniz.");
                }

            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata meydana geldi" + hata.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value.ToString()));
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Form1 gonder = new Form1();
            this.Hide();
            gonder.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tabloTiklamaKontrol(object sender, DataGridViewCellEventArgs e)
        {
            textBox4.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
        }

        private void ayarlarSayfasiGonder(object sender, EventArgs e)
        {
           Menuler.Ayarlar gonder = new Menuler.Ayarlar();
            this.Hide();
            gonder.Show();
        }

        private void musterilerSayfasiGonder(object sender, EventArgs e)
        {
            Menuler.Musteriler gonder = new Menuler.Musteriler();
            this.Hide();
            gonder.Show();
        }

        private void txt_sadece_sayi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txt_sadece_harf_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)
                && !char.IsSeparator(e.KeyChar);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Form1 gonderr = new Form1();
            liste gonder = new liste();
            this.Hide();
            gonder.Show();
        }

        private void kodlar_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 anaSyf = new Form1();
            anaSyf.Close();
            Application.ExitThread();
            Application.Exit();
        }
    }

}
