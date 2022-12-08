using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Net;
using System.Security;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.ComponentModel.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Borsa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Create empty collection/datasource

        }
        string user = System.Environment.UserName;
        bool lastRecordButtonControl = false;
        public bool mauseClick = false;

        public int clickedRow;
        public int unClickedRow;

        public class Item
        {
            public int row;
            public string menkul;
            public string adet;
            public string maliyet;
            public string maliyetTutari;
            public string sonFiyat;
            public string piyasaTutari;
            public string kz;
            public string isaret;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable tablo = new DataTable();
            tablo.Columns.Add("Menkul", typeof(string));
            tablo.Columns.Add("Adet", typeof(string));
            tablo.Columns.Add("Maliyet", typeof(string));
            tablo.Columns.Add("Maliyet_Tutari", typeof(string));
            tablo.Columns.Add("Son_Fiyat", typeof(string));
            tablo.Columns.Add(" ", typeof(string));
            tablo.Columns.Add("Piyasa_Tutari", typeof(string));
            tablo.Columns.Add("K/Z", typeof(string));
            dataGridView1.DataSource = tablo;
            dataGridView1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            label3.Text = "";
            label4.Visible = false;
            dataGridView1.Size = new Size(839, 495);
            LoadJson("list");
        }

        public void LoadJson(string file)
        {
            string filePath = file == "list" ? "C:\\Users\\" + user + "\\Documents\\borsa\\\\dat\\list.json" : "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + file + ".json";
            if (file == "list") label5.Visible = false; else label5.Visible = true;
            //MessageBox.Show(filePath);
        
            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                    DataTable dataTable = (DataTable)dataGridView1.DataSource;
                    dataTable.Clear();

                    dynamic array = JsonConvert.DeserializeObject(json);
                    foreach (var item in array)
                    {

                        DataRow drToAdd = dataTable.NewRow();
                        drToAdd["Menkul"] = item.menkul;
                        drToAdd["adet"] = item.adet;
                        drToAdd["Maliyet"] = item.maliyet;
                        drToAdd["Maliyet_Tutari"] = item.maliyetTutari;
                        drToAdd["Son_Fiyat"] = item.sonFiyat;
                        drToAdd["Piyasa_Tutari"] = item.piyasaTutari;
                        drToAdd["K/Z"] = item.kz;

                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        if (Convert.ToString(item.isaret) == "up")
                        {
                            label3.Text = "↑";
                            label3.ForeColor = Color.Green;
                        }
                        else
                        {
                            label3.Text = "↓";
                            label3.ForeColor = Color.Red;
                        }
                    }
                }
            } else
            {
                errorMessage("Böyle bir kayıt bulunamadı.", 2000);
            }
            
            karZararHesapla(true);
            colorizedGrid();
        }

        private void saveJson()
        {

            string tarih = DateTime.Now.ToString("dd.MM.yyyy");
            string saat = DateTime.Now.ToString("H:mm:ss").Replace(":", ",");

            string filePath = "C:\\Users\\"+user+"\\Documents\\borsa\\dat\\list.json";
            string destinationFile = "C:\\Users\\"+user+"\\Documents\\borsa\\dat\\lastRecordDat\\" + tarih + " - " + saat + " lastRecord.json";
            //MessageBox.Show(destinationFile);
            if (File.Exists(filePath))
            {
                System.IO.File.Move(filePath, destinationFile);
                //    System.IO.Directory.Move(@"C:\Users\simse\Documents\borsa\dat\", @"C:\Users\simse\Documents\borsa\dat\lastRecordDat\");
            }

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string isaret = "[";
            string isaret2 = "]";
            sw.WriteLine(isaret);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                var menkul = dataGridView1.Rows[i].Cells[0];
                var adet = dataGridView1.Rows[i].Cells[1];
                var maliyet = dataGridView1.Rows[i].Cells[2];
                var maliyetTutari = dataGridView1.Rows[i].Cells[3];
                var sonfiyat = dataGridView1.Rows[i].Cells[4];
                var piyasaTutari = dataGridView1.Rows[i].Cells[6];
                var kz = dataGridView1.Rows[i].Cells[7];

                string menkulStr = menkul.Value != null ? menkul.Value.ToString() : "isimsiz";
                string adetStr = adet.Value != null ? adet.Value.ToString() : "0";
                string maliyetStr = maliyet.Value != null ? maliyet.Value.ToString() : "0";
                string maliyetTutariStr = maliyetTutari.Value != null ? maliyetTutari.Value.ToString() : "0";
                string sonFiyatStr = sonfiyat.Value != null ? sonfiyat.Value.ToString() : "0";
                string piyasaTutariStr = piyasaTutari.Value != null ? piyasaTutari.Value.ToString() : "0";
                string kzStr = kz.Value != null ? kz.Value.ToString() : "0";
                string kzİsaret = label3.Text != "↓" ? "up" : "down";

                if (menkul.Value != null && adet.Value != null && maliyet.Value != null && maliyetTutari.Value != null && sonfiyat.Value != null && piyasaTutari.Value != null && kz.Value != null)
                {
                    string veri = String.Format(@"{{""row"":""{0}"", ""isaret"":""{8}"", ""menkul"":""{1}"", ""adet"":""{2}"", ""maliyet"":""{3}"", ""maliyetTutari"":""{4}"", ""sonFiyat"":""{5}"", ""piyasaTutari"":""{6}"", ""kz"":""{7}"" }},", i, menkulStr, adetStr, maliyetStr, maliyetTutariStr, sonFiyatStr, piyasaTutariStr, kzStr, kzİsaret);
                    sw.WriteLine(veri);
                }


            }
            sw.WriteLine(isaret2);
            sw.Flush();
            sw.Close();
            fs.Close();

        }

        private void showLogFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(filePath);
            }
            else
            {
                errorMessage("Belirtilen kayıt bulunamıyor.", 2000);
            }
        }

        public void colorizedGrid()
        {
            for(int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null && dataGridView1.Rows[i].Cells[0].Value.ToString() != "")
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("-"))
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.IndianRed;
                        dataGridView1.Rows[i].Cells[7].Style.BackColor = Color.White;
                    } else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }
                }

                if (dataGridView1.Rows[i].Cells[7].Value != null && dataGridView1.Rows[i].Cells[7].Value.ToString() != "")
                {
                    if(dataGridView1.Rows[i].Cells[7].Value.ToString().Contains("-"))
                    {
                        dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.Red;
                    } else
                    {
                        dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.Green;
                    }
                }
            }
        }

        private void karZararHesapla(bool s)
        {
            
            double karZarar = Convert.ToDouble(label2.Text);
            double toplam = 0.0;
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            {
                if (dataGridView1.Rows[i].Cells[7].Value != DBNull.Value)
                {
                    try
                    {
                        toplam += Convert.ToDouble(dataGridView1.Rows[i].Cells[7].Value);
                    }
                    catch { }
                }
            }

            if (s != true)
            {
                if (karZarar != toplam)
                {
                    if (toplam < karZarar)
                    {
                        label3.Text = "↓";
                        label3.ForeColor = Color.Red;
                    }
                    else
                    {
                        label3.Text = "↑";
                        label3.ForeColor = Color.Green;
                    }
                }
            } 

            label2.Text = toplam.ToString();
        }

        private void listBoxReload()
        {
            listBox1.Items.Clear();
            string path = "C:\\Users\\"+user+"\\Documents\\borsa\\dat\\lastRecordDat";

            string[] files = System.IO.Directory.GetFiles(path);

            for (int j = files.Length; j > 0; j--)
            {
                string file = files[j-1];          
                listBox1.Items.Add(file.Split('\\').Last().Replace("lastRecord.json", ""));
            }

        }

        bool numberControl(string text)
        {
            foreach (char chr in text)
            {
                if (chr.ToString() != "-" && chr.ToString() != "," && chr.ToString() != "." && chr.ToString() != "+")
                {
                    if (!Char.IsNumber(chr)) return false;
                }
                return true;
            }
            return true;
        }

        void errorMessage(string message, int time)
        {
            if (message.Length >= 1)
            {
                timer1.Enabled = true;
                timer1.Interval = time;
                label4.Visible = true;
                label4.Text = message;
                label4.ForeColor = Color.Red;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
            var menkul = dataGridView1.CurrentRow.Cells[0];
            var adet = dataGridView1.CurrentRow.Cells[1];
            var maliyet = dataGridView1.CurrentRow.Cells[2];
            var maliyetTutari = dataGridView1.CurrentRow.Cells[3];
            var sonfiyat = dataGridView1.CurrentRow.Cells[4];
            var piyasaTutari = dataGridView1.CurrentRow.Cells[6];
            var kz = dataGridView1.CurrentRow.Cells[7];

            if (menkul.Value.ToString().Contains("-") != true)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.White;
              
                if (adet.Value.ToString() != "" && sonfiyat.Value.ToString() != "")
                {
                    if(numberControl(adet.Value.ToString()) && numberControl(sonfiyat.Value.ToString())) {
                        piyasaTutari.Value = (Convert.ToDouble(adet.Value) * Convert.ToDouble(sonfiyat.Value)).ToString();
                    } else
                    {
                        errorMessage("Adet veya son fiyat desteklenmeyen karakter içeriyor.", 2000);
                    }
                    
                }

                if (maliyetTutari.Value.ToString() != "" && adet.Value.ToString() != "")
                {
                    if (numberControl(maliyetTutari.Value.ToString()) && numberControl(adet.Value.ToString()))
                    {
                        var fixMaliyetTutari = maliyetTutari.Value.ToString().Replace("-", "");
                        maliyet.Value = Math.Round(Convert.ToDouble(fixMaliyetTutari) / Convert.ToDouble(adet.Value), 2).ToString();
                    } else
                    {
                        errorMessage("Adet veya maliyet desteklenmeyen karakter içeriyor.", 2000);
                    }
                }

                if (piyasaTutari.Value.ToString() != "" && maliyetTutari.Value.ToString() != "")
                {
                    if (numberControl(piyasaTutari.Value.ToString()) && numberControl(maliyetTutari.Value.ToString()))
                    {
                        kz.Value = Math.Round(Convert.ToDouble(piyasaTutari.Value) + Convert.ToDouble(maliyetTutari.Value), 2).ToString();

                        if (kz.Value.ToString().Contains("-"))
                        {
                            kz.Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            kz.Style.ForeColor = Color.Green;
                        }
                    } else
                    {
                        errorMessage("Piyasa tutarı veya maliyet tutarı desteklenmeyen karakter içeriyor.", 2000);
                    }
                }
            } else
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.IndianRed;
                dataGridView1.CurrentRow.Cells[7].Style.BackColor = Color.White;
            }
            karZararHesapla(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!lastRecordButtonControl)
            {
                lastRecordButtonControl = true;
                dataGridView1.Location = new Point(12, 63);
                dataGridView1.Size = new Size(660, 495);
                listBox1.Visible = true;
                panel3.Visible = true;
                listBoxReload();
            } else
            {
                panel3.Visible = false;
                lastRecordButtonControl = false;
                dataGridView1.Size = new Size(839, 495);
                listBox1.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                LoadJson(listBox1.SelectedItem.ToString() + "lastRecord");
            } else
            {
                errorMessage("Önce görmek istediğiniz kayıtı seçiniz.", 1500);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadJson("list");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveJson();
            listBoxReload();
            errorMessage("Kayıt edildi", 1000);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            label4.Visible = false;
            label4.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadJson("list");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
            if (listBox1.SelectedIndex != -1) showLogFile(filePath); else errorMessage("Lütfen görüntülemek istediğiniz kayıtı seçiniz.", 2000);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\list.json";
            if(File.Exists(filePath)) showLogFile(filePath);

        }

        /*
        private void button8_Click(object sender, EventArgs e)
        {
            Process.Start("cmd.exe", "/k" + "cd C:\\borsaApp&git checkout main&git reset --hard").Close();
            Process.Start("cmd.exe", "/k" + "cd C:\\borsaApp&git pull").Close();

            Form1 form = new Form1();
            form.Close();
            Application.ExitThread();
            Application.Exit();

            Process.Start("cmd.exe", "/k" + "start C:\\borsaApp\\setup.exe").Close();

        }
        */

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (mauseClick)
            {
                mauseClick = false;
                unClickedRow = e.RowIndex;
               // label6.Text = "İlk tıklanan satır: " + clickedRow + " Son tıklanan satır: " + unClickedRow;

                var selectedRow = dataGridView1.Rows[clickedRow];
                var changedRow = dataGridView1.Rows[unClickedRow];

                if(dataGridView1.Rows.Count - 1 == selectedRow.Index || dataGridView1.Rows.Count - 1 == changedRow.Index)
                {
                    errorMessage("Hatalı tıklama", 1000);
                    return;
                }
                //MessageBox.Show((dataGridView1.Rows.Count - 1).ToString() + " " + selectedRow.Index);

                if (selectedRow.Cells[0].Value != null && changedRow.Cells[0].Value != null)
                {
                    var menkul = dataGridView1.Rows[clickedRow].Cells[0].Value.ToString();
                    var adet = dataGridView1.Rows[clickedRow].Cells[1].Value.ToString();
                    var maliyet = dataGridView1.Rows[clickedRow].Cells[2].Value.ToString();
                    var maliyetTutari = dataGridView1.Rows[clickedRow].Cells[3].Value.ToString();
                    var sonfiyat = dataGridView1.Rows[clickedRow].Cells[4].Value.ToString();
                    var piyasaTutari = dataGridView1.Rows[clickedRow].Cells[6].Value.ToString();
                    var kz = dataGridView1.Rows[clickedRow].Cells[7].Value.ToString();
                   // MessageBox.Show(changedRow.Cells[7].Value.ToString());
                    selectedRow.SetValues(changedRow.Cells[0].Value, changedRow.Cells[1].Value, changedRow.Cells[2].Value, changedRow.Cells[3].Value, changedRow.Cells[4].Value, " ",changedRow.Cells[6].Value, changedRow.Cells[7].Value);

                    changedRow.SetValues(menkul, adet, maliyet, maliyetTutari, sonfiyat, " ", piyasaTutari, kz);
                    colorizedGrid();

                }
            }

        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!mauseClick) mauseClick = true; clickedRow = e.RowIndex;
        }
    }
}
