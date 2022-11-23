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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;



namespace ManavYönetim
{


    public partial class liste : Form
    {
        public liste()
        {
            InitializeComponent();
        }

        static string constring = "Data Source=DESKTOP-FDFUFH4;Initial Catalog=manav;Integrated Security=True;MultipleActiveResultSets=True";
        string dosyaKonum = Form1.dosyaKonum;

        internal void listeYazdir_Click(DataGridView dataGridView1, object v)
        {
            throw new NotImplementedException();
        }

        // SqlConnection baglanti = new SqlConnection(@"Server=.;Database=;uid=sa;password=;MultipleActiveResultSets=True");
        SqlConnection baglanti = new SqlConnection(constring);
        DataTable tablo = new DataTable();
        string User = SystemInformation.UserName;

        int toplamFiyat;
        int genelToplamFiyat;
        int menuKontrol;
  
        bool suruklenmedurumu = false;
        Point ilkkonum;

        bool suruklenmeDurumListeKayit = false;
        Point ilkKonumListeKayit;

        Boolean yazdirMenuDurum = false;
        Boolean listeKayitMenuDurum = false;
        Boolean deisim;
        Boolean toplamaDurum;
        Boolean renk = false;

        string eskiVeri;



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

        private void comboBoxYenile()
        {
            comboBox1.Text = "";
            comboBox1.Items.Clear();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "SELECT * FROM kayitliTablolar";
            komut.Connection = baglanti;
            komut.CommandType = CommandType.Text;

            SqlDataReader dr;
            baglanti.Open();
            dr = komut.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["tabloIsim"]);
            }
            dr.Close();
            baglanti.Close();

        }

        private void musteriComboBoxYenile()
        {
            try
            {
                string[] klasorler = Directory.GetDirectories(dosyaKonum + "listeler");
                //klasörler dizisinin uzunluğuna kadar git
                for (int j = 0; j < klasorler.Length; j++)
                {
                    //klasörler dizisinin i. elemanı listboxa ekle
                    string dosya = klasorler[j];
                    string dosyaIsim = dosya.Split('\\').Last();
                    if (dosyaIsim != "musteriSecilmedi")
                    {
                        comboBox2.Items.Add(dosyaIsim);
                    }

                }
            } catch
            {
                MessageBox.Show("Müşteriler çekilemedi dosya bulunamamış olabilir");
            }


        }

        private void uyariBox(string yazi, int sure)
        {
            timer1.Interval = sure;
            label25.Text = yazi;
            System.Media.SystemSounds.Beep.Play();

            //Point ll = Point(panel16.Size(15, 5));
            label25.Visible = true;
            timer1.Enabled = true;
        }

        private void liste_Load(object sender, EventArgs e)
        {

            string tarih = DateTime.Now.ToString("dd.MM.yyyy");
            string saat = DateTime.Now.ToString("H:mm:ss");

            textBox17.Text = tarih;
            textBox18.Text = saat;
            textBox16.Text = "Ürün Listesi";

            tarihCheck.Checked = true;
            saatCheck.Checked = true;

            checkBox1.Checked = false;
            BaslikCheck.Checked = true;
            comboBox2.Enabled = false;

            comboBoxYenile();
            musteriComboBoxYenile();
            //textBox2.Enabled = false;
            //textBox2.Text = "Numara Bulunamadı";
            //button1.Enabled = false;

            textBox6.Enabled = false;
            textBox5.Enabled = false;
            textBox10.Enabled = false;

            tablo.Columns.Add("Numara", typeof(string));
            tablo.Columns.Add("İsim", typeof(string));
            tablo.Columns.Add("Parça Sayısı", typeof(string));
            tablo.Columns.Add("Kilo", typeof(string));
            tablo.Columns.Add("Fiyat", typeof(string));

            tablo.Columns.Add("Toplam Tutar", typeof(string));


            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.DataSource = tablo;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            //dataGridView1.Columns["Numara"].DefaultCellStyle.Format = "N2"; //N2 numara demek
            // bilgiMesaji("Bilgi", "Listede ekli olan ürünleri yazdırır.", button4);
            //bilgiMesaji("Bilgi", "Seçili olan ürünü listeden siler.", button3);
            //bilgiMesaji("Bilgi", "Seçtiğiniz ürünü değiştir.", button2);

        }

        private void urunEkleButton(object sender, EventArgs e)
        {

            string isim = textBox2.Text;
            int numara;
            string parcaSayisi = textBox12.Text;
            int fiyat;
            int kilo;

            if (textBox1.TextLength >= 1)
            {
                numara = Convert.ToInt32(textBox1.Text);
            }
            else
            {
                numara = 0;
            }

            if (textBox4.TextLength >= 1)
            {
                fiyat = Convert.ToInt32(textBox4.Text);
            }
            else
            {
                fiyat = 0;

            }

            if (textBox3.TextLength >= 1)
            {
                kilo = Convert.ToInt32(textBox3.Text);
            }
            else
            {
                kilo = 0;
            }

            if (textBox12.TextLength >= 1)
            {
                parcaSayisi = textBox12.Text;
            }
            else
            {
                parcaSayisi = "0";
            }

            if (textBox5.TextLength >= 1)
            {
                toplamFiyat = toplamFiyat;
            }
            else
            {
                toplamFiyat = 0;
            }

            tablo.Rows.Add(Convert.ToString(numara), isim, parcaSayisi, Convert.ToString(kilo), Convert.ToString(fiyat), Convert.ToString(toplamFiyat));
            dataGridView1.Refresh();



            int toplam = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            {
                toplam += Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
            }
            textBox15.Text = Convert.ToString(toplam);
            genelToplamFiyat = toplam;




        }

        private void kodYaziDegisim(object sender, EventArgs e)
        {
            int num1;

            if (!Int32.TryParse(textBox1.Text, out num1))
            {
                // MessageBox.Show(textBox1.Text);
                return;
            }

            int num2;

            if (!Int32.TryParse(textBox14.Text, out num2))
            {
                if(baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }

                SqlCommand komut = new SqlCommand();
                komut.CommandText = "select * from kodlar where kod=" + textBox1.Text;
                komut.Connection = baglanti;
                komut.CommandType = CommandType.Text;

                SqlDataReader dr;
                Boolean datalsAvailable = false;
                dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    textBox2.Text = Convert.ToString(dr["isim"]);
                    textBox2.Enabled = true;
                    button1.Enabled = true;
                    datalsAvailable = true;

                }
                dr.Close();
                if (!datalsAvailable)
                {
                    textBox2.Enabled = false;
                    textBox2.Text = "Numara Bulunamadı";
                    button1.Enabled = false;
                }
                baglanti.Close();


                //MessageBox.Show(DEBUG TEST);
                return;
            }

        }

        private void fiyatToplama(object sender, EventArgs e)
        {


            int num1;

            if (!Int32.TryParse(textBox14.Text, out num1))
            {

                int fiyat;
                if (textBox4.TextLength >= 1)
                {
                    fiyat = Convert.ToInt32(textBox4.Text);

                }
                else
                {
                    fiyat = 0;
                }

                int kilo;


                if (textBox3.TextLength >= 1)
                {
                    kilo = Convert.ToInt32(textBox3.Text);
                }
                else
                {
                    kilo = 0;
                }


                toplamFiyat = kilo * fiyat;


                textBox5.Text = Convert.ToString(toplamFiyat);
                return;

            }

            int num2;

            if (!Int32.TryParse(textBox11.Text, out num2))
            {

                // MessageBox.Show("sa");
                return;
            }

        }

        public void tabloKontrol(int stun, int satır, string eskiV, string yeniVeri)
        {
            //MessageBox.Show("s1");
            if (stun == 0 || stun == 2 || stun == 3 || stun == 4)
            {
                //MessageBox.Show("s");
                bool sayiKntrl(string text)
                {
                    foreach (char chr in text)
                    {
                        if (!Char.IsNumber(chr)) return false;
                    }
                    return true;
                }

                if (sayiKntrl(yeniVeri) == true)
                {

                    try
                    {
                        int toplam;
                        int fiyat = Convert.ToInt32(dataGridView1.Rows[satır].Cells[4].Value);
                        int kilo = Convert.ToInt32(dataGridView1.Rows[satır].Cells[3].Value);

                        if (fiyat >= 1)
                        {
                            fiyat = fiyat;
                        }
                        else
                        {
                            fiyat = 0;
                        }

                        if (kilo >= 1)
                        {
                            kilo = kilo;
                        }
                        else
                        {
                            kilo = 1;
                        }

                        toplam = kilo * fiyat;
                        dataGridView1.Rows[satır].Cells[5].Value = toplam;
                        //   MessageBox.Show(Convert.ToString(toplam));
                        toplamaDurum = true;
                        deisim = true;

                        int genelToplam = 0;
                        for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                        {
                            genelToplam += Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                        }
                        textBox15.Text = Convert.ToString(genelToplam);
                        genelToplamFiyat = genelToplam;

                        return;
                    }
                    catch { }
                }
                else
                {
                    
                    deisim = true;
                    uyariBox("Bu bölgeye sayı girilmesi gerekmektedir", 2000);
                    //dataGridView1.Rows[satır].Cells[stun].Value = Convert.ToInt32(eskiVeri);
                    //MessageBox.Show("Bu bölgeye sayı girilmemesi gerekmektedir");

                    return;


                }

            }
            return;
        }

        private void tabloTiklamaKontrol(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                int satır = e.RowIndex;
                int stun = e.ColumnIndex;

                eskiVeri = dataGridView1.Rows[satır].Cells[stun].Value.ToString();
                //textBox1.Text = eskiVeri;


                textBox10.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                textBox9.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                textBox13.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                textBox8.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                textBox7.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                textBox6.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();



                button2.Enabled = true;
                button3.Enabled = true;

            }
            catch { }
        }

        private void tabloDeisim(object sender, DataGridViewCellEventArgs e)
        {

            
            int satır = e.RowIndex;
            int stun = e.ColumnIndex;
            string yeniVeri = dataGridView1.Rows[satır].Cells[stun].Value.ToString();

            if (!deisim)
            {
                tabloKontrol(stun, satır, eskiVeri, yeniVeri);
                deisim = false;
            }

            if (stun == 0)
            {

                if (baglanti.State == ConnectionState.Closed)
                {
                    baglanti.Open();
                }


                SqlCommand komut = new SqlCommand();
                komut.CommandText = "select * from kodlar where kod=" + yeniVeri;
                komut.Connection = baglanti;
                komut.CommandType = CommandType.Text;

                SqlDataReader dr;
                try
                {
                    dr = komut.ExecuteReader();
                    while (dr.Read())
                    {
                        //MessageBox.Show(Convert.ToString(dr["isim"]));
                        dataGridView1.Rows[satır].Cells[1].Value = Convert.ToString(dr["isim"]);
                        // textBox2.Text = Convert.ToString(dr["isim"]);
                        // dataGridView1.Rows[satır].Cells[stun].Value = Convert.ToString(dr["isim"]);

                    }

                    dr.Close();
                    baglanti.Close();
                }
                catch { }

            }
            // MessageBox.Show("Eski veri = " + eskiVeri + " Yeni veri: " + dataGridView1.Rows[satır].Cells[stun].Value.ToString());
        }

        private void urunDuzenleButton(object sender, EventArgs e)
        {
            if (textBox7.TextLength >= 1 && textBox8.TextLength >= 1 && textBox9.TextLength >= 1)
            {
               
                dataGridView1.CurrentRow.Cells[1].Value = Convert.ToString(textBox9.Text);
                dataGridView1.CurrentRow.Cells[2].Value = Convert.ToString(textBox13.Text);
                dataGridView1.CurrentRow.Cells[3].Value = Convert.ToInt32(textBox8.Text);
                dataGridView1.CurrentRow.Cells[4].Value = Convert.ToInt32(textBox7.Text);
                dataGridView1.CurrentRow.Cells[5].Value = Convert.ToInt32(textBox6.Text);

                int toplam = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    toplam += Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                }

                textBox15.Text = Convert.ToString(toplam);
                genelToplamFiyat = toplam;

                button2.Enabled = false;
                button3.Enabled = false;

                textBox9.Text = "";
                textBox13.Text = "";
                textBox8.Text = "";
                textBox7.Text = "";
                textBox6.Text = "";
                textBox10.Text = "";
            }
            else
            {
                MessageBox.Show("Lütfen tüm bilgileri giriniz.", "Uyarı!");
            }
        }

        private void editFiyatToplam(object sender, EventArgs e)
        {

            int num1;

            if (!Int32.TryParse(textBox11.Text, out num1))
            {
                int fiyat;
                int kilo;

                if (textBox7.TextLength >= 1)
                {
                    fiyat = Convert.ToInt32(textBox7.Text);
                }
                else
                {
                    fiyat = 0;

                }

                if (textBox8.TextLength >= 1)
                {
                    kilo = Convert.ToInt32(textBox8.Text);
                }
                else
                {
                    kilo = 0;
                }


                toplamFiyat = kilo * fiyat;

                textBox6.Text = Convert.ToString(toplamFiyat);
                //   MessageBox.Show("ss");
                return;
            }

            int num2;

            if (!Int32.TryParse(textBox11.Text, out num2))
            {
                /*
                int fiyat = Convert.ToInt32(textBox8.Text);
                int kilo;


                if (textBox9.TextLength >= 1)
                {
                    kilo = Convert.ToInt32(textBox10.Text);
                }
                else
                {
                    kilo = 0;
                }


                toplamFiyat = kilo * fiyat;

                textBox12.Text = Convert.ToString(toplamFiyat);
                //MessageBox.Show(textBox1.Text);
                */
                // MessageBox.Show("s");
                return;
            }

        }

        private void urunSilButton(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                //MessageBox.Show(dataGridView1.CurrentRow.ToString() + " "+Convert.ToString(dataGridView1.RowCount-2));
                //MessageBox.Show(TabloSatir.ToString());


                try
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                } catch {
                    uyariBox("Bu satır silinemez", 2000);
                    //MessageBox.Show("Bu satır silinemez.", "Hata");
                }
                

                int toplam = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    try
                    {
                        toplam += Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                    } catch { }
                }
                textBox15.Text = Convert.ToString(toplam);
                genelToplamFiyat = toplam;

                textBox9.Text = "";
                textBox13.Text = "";
                textBox8.Text = "";
                textBox7.Text = "";
                textBox6.Text = "";
                textBox10.Text = "";
            }
            else
            {
                MessageBox.Show("Lüffen silinecek satırı seçin.");
            }
        }

        private void ListeKayitEtButton(object sender, EventArgs e)
        {
            if(textBox11.TextLength < 1)
            {
                MessageBox.Show("Tablo isimi boş bırakılamaz.", "Uyarı");
                return;
            }
            baglanti.Open();
            string urunNo;
            string urunIsım;
            string urunKilo;
            string urunFiyat;
            string toplamFiyat;
            string parcaSayisi;
            bool tabloVarmi = false;
            string tabloIsim = textBox11.Text;

            string tarih = DateTime.Now.ToString("dd.MM.yyyy H:mm:ss");



            int kayitKntrl = dataGridView1.Rows.Count;

            if (kayitKntrl >= 2)
            {

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM kayitliTablolar";
                cmd.Connection = baglanti;
                cmd.CommandType = CommandType.Text;

                SqlDataReader dr;

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (tabloIsim == Convert.ToString(dr["tabloIsim"]))
                    {
                        tabloVarmi = true;
                    }

                }
                dr.Close();

                if (!tabloVarmi)
                {

                }
                else
                {
                    MessageBox.Show("Bu isimde mevcut kayıtlı liste bulunuyor.", "Uyarı");
                    if (baglanti.State == ConnectionState.Open)
                    {
                        baglanti.Close();
                    }
                    return;
                }

                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {

                    if (i < kayitKntrl - 1)
                    {
                        urunNo = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);
                        urunIsım = Convert.ToString(dataGridView1.Rows[i].Cells[1].Value);
                        parcaSayisi = Convert.ToString(dataGridView1.Rows[i].Cells[2].Value);
                        urunKilo = Convert.ToString(dataGridView1.Rows[i].Cells[3].Value);
                        urunFiyat = Convert.ToString(dataGridView1.Rows[i].Cells[4].Value);
                        toplamFiyat = Convert.ToString(dataGridView1.Rows[i].Cells[5].Value);

                        string kayit = "insert into kayitliTablo (tabloIsim,urunKod,urunIsim,parcaSayisi, urunKilo,urunFiyat,toplamFiyat) values(@tabloIsim, @urunKod,@urunIsim, @parcaSayisi, @urunKilo,@urunFiyat,@toplamFiyat)";
                        SqlCommand komut = new SqlCommand(kayit, baglanti);

                        komut.Parameters.AddWithValue("@tabloIsim", tabloIsim);
                        komut.Parameters.AddWithValue("@urunKod", urunNo);
                        komut.Parameters.AddWithValue("@urunIsim", urunIsım);
                        komut.Parameters.AddWithValue("@parcaSayisi", parcaSayisi);
                        komut.Parameters.AddWithValue("@urunKilo", urunKilo);
                        komut.Parameters.AddWithValue("@urunFiyat", urunFiyat);
                        komut.Parameters.AddWithValue("@ToplamFiyat", toplamFiyat);
                        komut.ExecuteNonQuery();
                    }



                }
                string kayit2 = "insert into kayitliTablolar (tabloIsim, kayitTarih) values(@tabloIsimi, @tarih)";
                SqlCommand komut2 = new SqlCommand(kayit2, baglanti);
                komut2.Parameters.AddWithValue("@tabloIsimi", tabloIsim);
                komut2.Parameters.AddWithValue("@tarih", tarih);
                komut2.ExecuteNonQuery();
                MessageBox.Show("Tablo Kayıt Edildi");
                baglanti.Close();
                comboBoxYenile();

            }
            else
            {
                MessageBox.Show("Öncelikle tabloya veri eklemelisiniz.");
                if(baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();
                }
            }


        }

        private void ListeSilButton(object sender, EventArgs e)
        {

            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();

            }

            SqlCommand komut = new SqlCommand("delete from kayitliTablo where TabloIsim=@listeAd", baglanti);
            SqlCommand komut2 = new SqlCommand("delete from kayitliTablolar where TabloIsim=@listeAd", baglanti);
            komut.Parameters.AddWithValue("@listeAd", comboBox1.Text);
            komut2.Parameters.AddWithValue("@listeAd", comboBox1.Text);
            komut.ExecuteNonQuery();
            komut2.ExecuteNonQuery();
            baglanti.Close();
            comboBoxYenile();

        }

        private void listeGetirButton(object sender, EventArgs e)
        {
            if (baglanti.State == ConnectionState.Closed)
            {

                baglanti.Open();

            }

            string srg = comboBox1.Text;
            string sorgu = "Select urunKod,urunIsim, parcaSayisi, urunKilo,urunFiyat,toplamFiyat from kayitliTablo where tabloIsim Like '" + srg + "'";
            SqlDataAdapter adap = new SqlDataAdapter(sorgu, baglanti);
            //   dataGridView1.Columns[0].HeaderText = "ss";

            DataSet ds = new DataSet();
            adap.Fill(ds, "kayit");
            tablo = ds.Tables[0];
            this.dataGridView1.DataSource = tablo;


            baglanti.Close();
            int veri = 0;
            int toplam = 0;
            for (int i = 0;i <= dataGridView1.RowCount; i++)
            {
                try
                {
                    
                    int sayi = Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value);
                    toplam = sayi + veri;
                    veri = toplam;
                    
                } catch { }
            }
            textBox15.Text = toplam.ToString();
        }

        private void txt_sadece_sayi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void anaMenuGonder(object sender, EventArgs e)
        {
            DialogResult uyari = MessageBox.Show("Eğer bu sekmeyi kapatırsanız hazırladığınız liste silinir", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (uyari == DialogResult.Yes)
            {
                Form1 gonder = new Form1();
                this.Hide();
                gonder.Show();
            }
        }

        private void kodlarSayfasiGonder(object sender, EventArgs e)
        {
            DialogResult uyari = MessageBox.Show("Eğer bu sekmeyi kapatırsanız hazırladığınız liste silinir", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (uyari == DialogResult.Yes)
            {
                kodlar gonder = new kodlar();
                this.Hide();
                gonder.Show();
            }
        }

        private void musterilerSayfasiGonder(object sender, EventArgs e)
        {
            DialogResult uyari = MessageBox.Show("Eğer bu sekmeyi kapatırsanız hazırladığınız liste silinir", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if(uyari == DialogResult.Yes)
            {
                Menuler.Musteriler gonder = new Menuler.Musteriler();
                this.Hide();
                gonder.Show();
            } 


        }

        private void ayarlarSayfasiGonder(object sender, EventArgs e)
        {
            DialogResult uyari = MessageBox.Show("Eğer bu sekmeyi kapatırsanız hazırladığınız liste silinir", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (uyari == DialogResult.Yes)
            {
                Menuler.Ayarlar gonder = new Menuler.Ayarlar();
                this.Hide();
                gonder.Show();
            }
        }

        private void listeYazdir(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                string olusturmaTarih = DateTime.Now.ToString("dd.MM.yyyy H;mm;ss");
                string dosyaIsim = "Liste";
                SaveFileDialog save = new SaveFileDialog();
                save.RestoreDirectory = false;
                save.Filter = "PDF (*.pdf)|*.pdf";

                if(textBox19.TextLength >= 1)
                {
                    dosyaIsim = (textBox19.Text+".pdf");
                } else
                {
                    dosyaIsim = "Liste.pdf";
                }

                if(checkBox1.Checked == true)
                {
                    
                    save.FileName = dosyaKonum+"Listeler\\"+comboBox2.Text+"\\" + olusturmaTarih + " - "+ dosyaIsim;
                } else
                {
                    save.FileName = dosyaKonum+"Listeler\\musteriSecilmedi\\" + olusturmaTarih + " - " +dosyaIsim;
                }
                




                bool ErrorMessage = false;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(save.FileName))
                    {
                        try
                        {
                            File.Delete(save.FileName);
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage = true;
                            MessageBox.Show("Diske veri yazılamıyor" + ex.Message);
                        }
                    }
                    if (!ErrorMessage)
                    {
                        try
                        {

                            PdfPTable pTable = new PdfPTable(5);
                            pTable.DefaultCell.Padding = 2;
                            pTable.WidthPercentage = 75;
                            pTable.HorizontalAlignment = Element.ALIGN_CENTER;

                            /*foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                PdfPCell pCell = new PdfPCell(new Phrase(col.HeaderText));
                                pTable.AddCell(pCell);
                            }*/

                            //pTable.AddCell("Kod");
                            iTextSharp.text.pdf.BaseFont STF_Helvetica_Turkish = iTextSharp.text.pdf.BaseFont.CreateFont("Helvetica", "CP1254", iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
                            iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(STF_Helvetica_Turkish, 10, iTextSharp.text.Font.NORMAL);

                            pTable.AddCell("Isim");
                            pTable.AddCell("Parca Sayisi");
                            pTable.AddCell("Kilo");
                            pTable.AddCell("Fiyat");
                            pTable.AddCell("Toplam Fiyat");

                            PdfPCell cell = null;

                            foreach (DataGridViewRow viewRow in dataGridView1.Rows)
                            {
                                Color re = System.Drawing.Color.Blue;
                                //MessageBox.Show(Convert.ToString(viewRow.Index));

                                if (!renk)
                                {
                                    re = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                    renk = true;
                                }
                                else
                                {
                                    re = System.Drawing.ColorTranslator.FromHtml("#C0C0C0");
                                    renk = false;
                                }

                                foreach (DataGridViewCell dcell in viewRow.Cells)
                                {

                                    if (dcell.Value != null)
                                    {
                                        //if (dataGridView1.Rows[satir].Cells[dscell.ColumnIndex])
                                        //MessageBox.Show(Convert.ToString(dataGridView1.Columns[dcell.ColumnIndex].HeaderText));

                                        if (Convert.ToString(dataGridView1.Columns[dcell.ColumnIndex].HeaderText) != "Numara" && Convert.ToString(dataGridView1.Columns[dcell.ColumnIndex].HeaderText) != "urunKod")
                                        {
                                            //MessageBox.Show("Numara değil");
                                            //MessageBox.Show(dcell.Value.ToString());
                                            //MessageBox.Show(Convert.ToString(renk));
                                            //MessageBox.Show(Convert.ToString(dataGridView1.Columns[dcell.ColumnIndex].HeaderText));


                                            cell = new PdfPCell(new Phrase(dcell.Value.ToString(), fontTitle));
                                            cell.BackgroundColor = new BaseColor(re);


                                            pTable.AddCell(cell);

                                        }
                                        else
                                        {
                                            //MessageBox.Show("numara");
                                        }
                                    }
                                }

                            }

                            using (FileStream fileStream = new FileStream(save.FileName, FileMode.Create))
                            {
                                int borc = 0;
                                if(baglanti.State == ConnectionState.Closed)
                                {
                                    baglanti.Open();
                                }

                                try
                                {
                                    if (checkBox1.Checked == true)
                                    {

                                        SqlCommand cmd2 = new SqlCommand("select * from musteriler where musteri=@veri2", baglanti);
                                        cmd2.Parameters.AddWithValue("@veri2", comboBox2.Text);
                                        SqlDataReader dr = cmd2.ExecuteReader();
                                        if (dr.Read())
                                        {
                                            borc = Convert.ToInt32(dr["borc"]);
                                        }
                                        else
                                        {
                                            borc = 0;
                                        }
                                        dr.Close();
                                        baglanti.Close();
                                    }
                                    else
                                    {
                                        borc = 0;
                                        baglanti.Close();
                                    }
                                }
                                catch
                                {
                                    MessageBox.Show("Müşteriler çekilirken bir hata oluştu dosya bulunamamış olabilir");
                                    baglanti.Close();
                                    borc = 0;
                                }
                                

                                Document document = new Document(PageSize.A4, 8f, 16f, 16f, 8f);

                                PdfWriter.GetInstance(document, fileStream);
                                document.Open();

                                var tarihFont = FontFactory.GetFont("Arial", 8, BaseColor.BLACK);
                                var baslikFont = FontFactory.GetFont("Arial", 14, BaseColor.BLACK);

                                Paragraph tarih = new Paragraph(new Chunk("Tarih: " + textBox17.Text, tarihFont));
                                Paragraph saat = new Paragraph(new Chunk("Saat: " + textBox18.Text, tarihFont));
                                tarih.Alignment = Element.ALIGN_LEFT;
                                saat.Alignment = Element.ALIGN_LEFT;

                                var font = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);

                                Paragraph toplamFiyat = new Paragraph("\n\nListe toplam fiyatı: " + textBox15.Text, font);
                                Paragraph veresiye = new Paragraph("Veresiye borcu: " + borc, font);

                                if(tarihCheck.Checked == true)
                                {
                                    document.Add(tarih);
                                }
                                
                                if(saatCheck.Checked == true)
                                {
                                    document.Add(saat);
                                }
                                document.Add(toplamFiyat);

                                if(checkBox1.Checked == true)
                                {
                                    if(checkBox3.Checked == true)
                                    {
                                        document.Add(veresiye);
                                    }
                                    
                                }
                                

                                Paragraph baslik = new Paragraph(textBox16.Text, baslikFont);
                                baslik.Alignment = Element.ALIGN_CENTER;

                                if(BaslikCheck.Checked == true)
                                {
                                    document.Add(baslik);
                                }

                                /*
                                Paragraph musteri = new Paragraph(comboBox2.Text, baslikFont);
                                musteri.Alignment = Element.ALIGN_CENTER;
                                if(checkBox1.Checked==true)
                                {
                                    document.Add(musteri);
                                }
                                */

                                document.Add(new Paragraph("\n\n"));

                                document.Add(pTable);


                                document.Close();
                                fileStream.Close();

                                System.Diagnostics.Process.Start(save.FileName);
                            }
                            //MessageBox.Show("PDF yazıldı");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Verileri dışa aktarırken hata oluştu" + ex.Message);
                        }

                    }

                }

            }
            else
            {
                MessageBox.Show("Veri kayıdı bulunamadı", "Uyarı!");
            }
        }

        private void panel6_MouseDown(object sender, MouseEventArgs e)
        {

            suruklenmedurumu = true;
            panel6.Cursor = Cursors.SizeAll;
            ilkkonum = e.Location;
        }

        private void panel6_MouseMove(object sender, MouseEventArgs e)
        {
            if (suruklenmedurumu)
            {
                panel6.Left = e.X + panel6.Left - (ilkkonum.X);

                panel6.Top = e.Y + panel6.Top - (ilkkonum.Y);
            }
        }

        private void panel6_MouseUp(object sender, MouseEventArgs e)
        {
            suruklenmedurumu = false;
            panel6.Cursor = Cursors.Default;
        }

        private void panel9_MouseDown(object sender, MouseEventArgs e)
        {

            suruklenmeDurumListeKayit = true;
            panel9.Cursor = Cursors.SizeAll;
            ilkKonumListeKayit = e.Location;
        }

        private void panel9_MouseMove(object sender, MouseEventArgs e)
        {
            if (suruklenmeDurumListeKayit)
            {
                panel9.Left = e.X + panel9.Left - (ilkKonumListeKayit.X);

                panel9.Top = e.Y + panel9.Top - (ilkKonumListeKayit.Y);
            }
        }

        private void panel9_MouseUp(object sender, MouseEventArgs e)
        {
            suruklenmeDurumListeKayit = false;
            panel9.Cursor = Cursors.Default;
        }

        private void BaslikCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (BaslikCheck.Checked == true)
            {
                textBox16.Enabled = true;
                textBox16.Text = "Ürün Listesi";
            }
            else
            {
                textBox16.Text = "";
                textBox16.Enabled = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                comboBox2.Enabled = true;

            }
            else
            {
                comboBox2.Text = "";
                
                comboBox2.Enabled = false;
            }
        }

        private void tarihCheck_CheckedChanged(object sender, EventArgs e)
        {
            string tarih = DateTime.Now.ToString("dd.MM.yyyy");

            if (tarihCheck.Checked == true)
            {
                textBox17.Enabled = true;
                textBox17.Text = tarih;
            }
            else
            {
                textBox17.Text = "";
                textBox17.Enabled = false;
            }
        }

        private void saatCheck_CheckedChanged(object sender, EventArgs e)
        {
            string saat = DateTime.Now.ToString("H:mm:ss");

            if (saatCheck.Checked == true)
            {
                textBox18.Enabled = true;
                textBox18.Text = saat;
            }
            else
            {
                textBox18.Text = "";
                textBox18.Enabled = false;
            }
        }

        private void yazdirMenuGetir(object sender, EventArgs e)
        {
            string saat = DateTime.Now.ToString("H:mm:ss");
            string tarih = DateTime.Now.ToString("dd.MM.yyyy");

            if (!yazdirMenuDurum)
            {
                panel6.Location = new Point(204, 177);
                yazdirMenuDurum = true;
                if(tarihCheck.Checked == true)
                {
                    textBox17.Text = tarih;
                }
                if(saatCheck.Checked == true)
                {
                    textBox18.Text = saat;
                }
                
            }
            else
            {
                panel6.Location = new Point(869, 115);
                yazdirMenuDurum = false;
            }
        }

        private void listeKayitMenuGetir(object sender, EventArgs e)
        {

            if (!listeKayitMenuDurum)
            {
                panel9.Location = new Point(325, 292);
                listeKayitMenuDurum = true;
            }
            else
            {
                panel9.Location = new Point(891, 351);
                listeKayitMenuDurum = false;
            }
        }

        private void saatKeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (char.IsNumber(e.KeyChar) || e.KeyChar == ';')
            {
                
            }
            else
            {
                MessageBox.Show("Bu bölüme sayı ve ; işareti dışında farklı bir karakter girilemez");
                e.Handled = true;
            }
        }

        private void tarihKeyPress(object sender, KeyPressEventArgs e)
        {

            if (char.IsNumber(e.KeyChar) || e.KeyChar == ';')
            {

            }
            else
            {
                MessageBox.Show("Bu bölüme sayı ve . işareti dışında farklı bir karakter girilemez");
                e.Handled = true;
            }
        }

        private void baslikKeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                if (!char.IsNumber(e.KeyChar))
                {
                    if(!char.IsWhiteSpace(e.KeyChar))
                    {
                        e.Handled = true;
                        MessageBox.Show("Başlığa işarat konulamaz");
                    }

                }

            }

        }

        private void liste_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 anaSyf = new Form1();
            anaSyf.Close();
            Application.ExitThread();
            Application.Exit();
        }

        private void uyariBox_Tick(object sender, EventArgs e)
        {
            label25.Visible = false;
            label25.Text = "";
            timer1.Enabled = false;
        }

        private void dataGridView1_CurrentCellChanged_1(object sender, EventArgs e)
        {
            MessageBox.Show("s");
        }
    }

}
