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
using System.Linq.Expressions;
using System.Security.Cryptography;
using NUnit.Framework.Interfaces;

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
        public class userOptns
        {
            public string balance;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            s.files();
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
            dataGridView1.Location = new Point(45, 76);
            dataGridView1.Size = new Size(839, 495);
            panel5.Location = new Point(640, 580);
            LoadJson("list");

            int balance = 0;
            string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\userOptions.json";
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                List<userOptns> options = JsonConvert.DeserializeObject<List<userOptns>>(json);
                dynamic array = JsonConvert.DeserializeObject(json);
                foreach (var option in array)
                {
                    balance = option.balance;
                }
            }
            textBox1.Text = balance.ToString();

            WriteLog("Uygulama baslatıldı");
        }

        public void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = "C:\\Users\\"+ user +"\\Documents\\borsa\\logs\\";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("dd-MM-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine( "[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "]: " + strLog);
            log.Close();
            errorMessage("Bir hata oluştu log dosyasını kontrol ediniz.", 3000);
        }

        public void LoadJson(string file)
        {
            try
            {
                string filePath = file == "list" ? "C:\\Users\\" + user + "\\Documents\\borsa\\\\dat\\list.json" : file;

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
                }
                else
                {
                    errorMessage("Böyle bir kayıt bulunamadı.", 2000);
                }

                karZararHesapla(true);
                colorizedGrid();
            } catch(Exception e)
            {
                WriteLog("Kayıt yüklenirken hata oluştu [" + file + "] " + e.Message);
            }
        }

        public void toplamBakiyeHesapla()
        {
            try
            {
                long balance = 0;
                string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\userOptions.json" ;
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    List<userOptns> options = JsonConvert.DeserializeObject<List<userOptns>>(json);
                    dynamic array = JsonConvert.DeserializeObject(json);
                    foreach (var option in array)
                    {
                        balance = option.balance;
                    }
                }

                long toplam = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    if (dataGridView1.Rows[i].Cells[6].Value != DBNull.Value)
                    {
                        if (dataGridView1.Rows[i].Cells[6].Value != "")
                        {
                        toplam += Convert.ToInt64(dataGridView1.Rows[i].Cells[6].Value);

                        }
                    }
                }
                label13.Text = (toplam + balance).ToString();


            } catch(Exception ex)
            {
                WriteLog("Toplam bakiye hesaplanırken bir sorun oluştu " + ex.Message);
            }
        }
        private void saveJson()
        {
            try
            {
                string tarih = DateTime.Now.ToString("dd.MM.yyyy");
                string saat = DateTime.Now.ToString("H:mm:ss").Replace(":", ",");

                string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\list.json";
                string destinationFile = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + tarih + " - " + saat + " lastRecord.json";
                //MessageBox.Show(destinationFile);
                if (File.Exists(filePath))
                {

                    //System.IO.File.Move(filePath, destinationFile);

                    //File.Create(destinationFile);
                    System.Threading.Thread.Sleep(1000);
                    File.Copy(filePath, destinationFile);
                    File.Delete(filePath);


                }

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine("[");
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
                sw.WriteLine("]");
                sw.Flush();
                sw.Close();
                fs.Close();
            } catch (Exception e)
            {
                WriteLog("Liste kayıt edilirken bir hata oluştu(saveJson) " + e.Message);
            }
        }
        public void balanceSaveToJson(long balance)
        {
            try
            {
                String fileLoc = "C:\\Users\\" + user + "\\Documents\\borsa\\userOptions.json";
                File.Delete(fileLoc);
                FileStream fs = new FileStream(fileLoc, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                String veri = String.Format(@"[{{""balance"": ""{0}""}}]", balance);
                sw.Write(veri);
                sw.Flush();
                sw.Close();
                fs.Close();
                toplamBakiyeHesapla();
            } catch(Exception ex)
            {
                WriteLog("Bakiye kayıt edilirken hata oluştu " + ex.Message);
            }
        }
        private void showLogFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(filePath);
                }
                else
                {
                    errorMessage("Belirtilen kayıt bulunamıyor.", 2000);
                }
            } catch (Exception e) {
                WriteLog("Kayıt açılırken bir sorun oluştu(ShowLogFile " + e.Message);
            }
        }

        public void colorizedGrid()
        {
            try
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != null && dataGridView1.Rows[i].Cells[0].Value.ToString() != "")
                    {
                        if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains("-"))
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.IndianRed;
                            dataGridView1.Rows[i].Cells[7].Style.BackColor = Color.White;
                        }
                        else
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    if (dataGridView1.Rows[i].Cells[7].Value != null && dataGridView1.Rows[i].Cells[7].Value.ToString() != "")
                    {
                        if (dataGridView1.Rows[i].Cells[7].Value.ToString().Contains("-"))
                        {
                            dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[7].Style.ForeColor = Color.Green;
                        }
                    }
                }
            } catch(Exception e)
            {
                WriteLog("Tablo renklendirilirken bir sorun oluştu(colorizedGrid) " + e.Message);
            }
        }

        private void karZararHesapla(bool s)
        {
            try
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
            } catch(Exception e)
            {
                WriteLog("Kar zarar hesaplanırken bir sorun oluştu(karZararHesapla) " + e.Message);
            }
        }

        private void listBoxReload()
        {
            try
            {
                listBox1.Items.Clear();
                string path = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat";

                string[] files = System.IO.Directory.GetFiles(path);

                for (int j = files.Length; j > 0; j--)
                {
                    string file = files[j - 1];
                    listBox1.Items.Add(file.Split('\\').Last().Replace("lastRecord.json", "").Replace(".json", ""));
                }
            } catch(Exception e)
            {
                WriteLog("Listbox yenilenirken bir sorun oluştu(listBoxReload) " + e.Message);
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
            try
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
                        if (numberControl(adet.Value.ToString()) && numberControl(sonfiyat.Value.ToString()))
                        {
                            piyasaTutari.Value = (Convert.ToDouble(adet.Value) * Convert.ToDouble(sonfiyat.Value)).ToString();
                        }
                        else
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
                        }
                        else
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
                        }
                        else
                        {
                            errorMessage("Piyasa tutarı veya maliyet tutarı desteklenmeyen karakter içeriyor.", 2000);
                        }
                    }
                }
                else
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.IndianRed;
                    dataGridView1.CurrentRow.Cells[7].Style.BackColor = Color.White;
                }
                karZararHesapla(false);
                toplamBakiyeHesapla();
            } catch (Exception ex)
            {
                WriteLog("CellValueChanged eventinde bir sorun oluştu " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!lastRecordButtonControl)
            {
                lastRecordButtonControl = true;
                dataGridView1.Location = new Point(12, 76);
                
                dataGridView1.Size = new Size(530, 498);
                listBox1.Visible = true;
                panel3.Visible = true;
                button8.Visible = true;
                panel5.Location = new Point(320, 577);
                button9.Visible = true;
                listBoxReload();
            }
            else
            {
                button8.Visible = false;
                button9.Visible = false;
                panel3.Visible = false;
                lastRecordButtonControl = false;
                panel5.Location = new Point(640, 580);
                dataGridView1.Location = new Point(45, 76);
                dataGridView1.Size = new Size(839, 495);
                listBox1.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    String file1 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
                    String file2 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + ".json";

                    String filePath = File.Exists(file1) == false ? file2 : file1;
                    LoadJson(filePath);
                    //MessageBox.Show(filePath);
                }
                else
                {
                    errorMessage("Önce görmek istediğiniz kayıtı seçiniz.", 1500);
                }
            } catch(Exception ex)
            {
                WriteLog("Button5 tıklama eventinde bir sorun oluştu " + ex.Message);
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
            
            if (listBox1.SelectedIndex != -1)
            {
                string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
                string filePath2 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + ".json";

                string file = File.Exists(filePath) == false ? filePath2 : filePath;
                showLogFile(file);
            }
            else errorMessage("Lütfen görüntülemek istediğiniz kayıtı seçiniz.", 2000);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\list.json";
            if (File.Exists(filePath)) showLogFile(filePath);

        }
        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (mauseClick)
            {
                mauseClick = false;
                unClickedRow = e.RowIndex;
                
                var selectedRow = dataGridView1.Rows[clickedRow];
                var changedRow = dataGridView1.Rows[unClickedRow];

                if (dataGridView1.Rows.Count - 1 == selectedRow.Index || dataGridView1.Rows.Count - 1 == changedRow.Index)
                {
                    
                    return;
                }

                if (selectedRow.Cells[0].Value != null && changedRow.Cells[0].Value != null)
                {
                    var menkul = dataGridView1.Rows[clickedRow].Cells[0].Value.ToString();
                    var adet = dataGridView1.Rows[clickedRow].Cells[1].Value.ToString();
                    var maliyet = dataGridView1.Rows[clickedRow].Cells[2].Value.ToString();
                    var maliyetTutari = dataGridView1.Rows[clickedRow].Cells[3].Value.ToString();
                    var sonfiyat = dataGridView1.Rows[clickedRow].Cells[4].Value.ToString();
                    var piyasaTutari = dataGridView1.Rows[clickedRow].Cells[6].Value.ToString();
                    var kz = dataGridView1.Rows[clickedRow].Cells[7].Value.ToString();

                    selectedRow.SetValues(changedRow.Cells[0].Value, changedRow.Cells[1].Value, changedRow.Cells[2].Value, changedRow.Cells[3].Value, changedRow.Cells[4].Value, " ", changedRow.Cells[6].Value, changedRow.Cells[7].Value);

                    changedRow.SetValues(menkul, adet, maliyet, maliyetTutari, sonfiyat, " ", piyasaTutari, kz);
                    colorizedGrid();
                    
                }
            }

        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!mauseClick) { mauseClick = true; clickedRow = e.RowIndex; }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
                    string filePath2 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + ".json";

                    string file = File.Exists(filePath) == false ? filePath2 : filePath;
                    if (File.Exists(file))
                    {
                        DialogResult message = MessageBox.Show("Seçili kayıtı silmek istediğinize emin misiniz(geri alınamaz)", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (message == DialogResult.Yes)
                        {
                            File.Delete(file);
                            listBoxReload();
                        }
                    }
                }
                else errorMessage("Silmek istediğiniz kayıtı seçmeniz gerekmektedir", 1500);
            } catch(Exception ex)
            {
                WriteLog("Button8 click eventinde bir sorun oluştu " + ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                String directoryPath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat";
                if (Directory.Exists(directoryPath))
                {
                    DialogResult message = MessageBox.Show("Tüm kayıtları silmek istediğinize emin misiniz(geri alınamaz)", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (message == DialogResult.Yes)
                    {

                        Directory.Delete(directoryPath, true);
                        System.Threading.Thread.Sleep(1500);
                        Directory.CreateDirectory(directoryPath);
                        listBoxReload();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Button9 click eventinde bir sorun oluştu " + ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
                string filePath2 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + ".json";

                string file = File.Exists(filePath) == false ? filePath2 : filePath;
                

                textBox2.Text = listBox1.SelectedItem.ToString();
                textBox3.Text = File.GetCreationTime(file).ToString();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    string filePath = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + "lastRecord.json";
                    string filePath2 = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + listBox1.SelectedItem.ToString() + ".json";

                    string file = File.Exists(filePath) == false ? filePath2 : filePath;

                    string renameFile = "C:\\Users\\" + user + "\\Documents\\borsa\\dat\\lastRecordDat\\" + textBox2.Text + ".json";
                    if (File.Exists(file))
                    {
                        if (!File.Exists(renameFile))
                        {
                            File.Move(file, renameFile);
                            listBoxReload();
                        }
                        else errorMessage("Zaten bu isimde bir kayıt bulunuyor", 2000);
                    }
                }
            } catch(Exception ex)
            {
                WriteLog("Button11 click eventinde bir sorun oluştu " + ex.Message);
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.TextLength >= 1)
            {
                balanceSaveToJson(Convert.ToInt64(textBox1.Text));
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}