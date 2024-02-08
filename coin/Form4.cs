using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.OleDb;
using Newtonsoft.Json;
using static coin.Form2;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;

// Enes AYDIN 20010207042
namespace coin
{
    public partial class Form4 : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader dr;
        OleDbDataAdapter adp;
        DataSet dataSet;
        private string kullaniciad;
        private string parola;

        public Form4(string kullaniciad, string parola)
        {
            InitializeComponent();
            this.kullaniciad = kullaniciad;
            this.parola = parola;
        }
        // form açılışı ile veri tabanındaki ilk verileri gösterme
        private void Form4_Load(object sender, EventArgs e)
        {
            label31.Text = "HOŞ GELDİN " + kullaniciad.ToUpper() ;
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
            cmd = new OleDbCommand();
            adp = new OleDbDataAdapter("Select * from " + kullaniciad, con);
            dataSet = new DataSet();
            con.Open();
            adp.Fill(dataSet, kullaniciad);
            dataGridView1.DataSource = dataSet.Tables[kullaniciad];
            con.Close();

            textBox3.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            textBox8.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox3.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            textBox8.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
        }
        // kullanıcı verilerini güncelleme
        async private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "" && textBox4.Text != "" && textBox5.Text != "" && 
                textBox6.Text != "" && textBox7.Text != "" && textBox8.Text != "")
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
                cmd = new OleDbCommand();
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = $"UPDATE {kullaniciad} SET btc = {Convert.ToInt32(textBox3.Text)}, " +
                                      $"eth = {Convert.ToInt32(textBox4.Text)}, " +
                                      $"doge = {Convert.ToInt32(textBox5.Text)}, " +
                                      $"chz = {Convert.ToInt32(textBox6.Text)}, " +
                                      $"trx = {Convert.ToInt32(textBox7.Text)}, " +
                                      $"xrp = {Convert.ToInt32(textBox8.Text)} " +
                                      $"WHERE kullanici = '{kullaniciad}'";
                cmd.ExecuteNonQuery();
                con.Close();
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
                cmd = new OleDbCommand();
                adp = new OleDbDataAdapter("Select * from " + kullaniciad, con);
                dataSet = new DataSet();
                con.Open();
                adp.Fill(dataSet, kullaniciad);
                dataGridView1.DataSource = dataSet.Tables[kullaniciad];
                con.Close();
            }
            else
            {
                MessageBox.Show("Boş ifade bırakmayınız");
            }
        }
        // api ile değer çekme (usdt)
        private static async Task<double> UsdtDegeri(string coin)
        {
            try
            {
                string apiUrl1 = "https://api.btcturk.com/api/v2/ticker?pairSymbol=" + coin + "usdt";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl1);
                    if (response.IsSuccessStatusCode)
                    {
                        ;
                        string responseData = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiDegerleri>(responseData);
                        double lastValue = apiResponse.Data[0].Last;
                        return lastValue;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        // api ile değer çekme (try)
        private static async Task<double> TryDegeri(string coin)
        {
            try
            {
                string apiUrl1 = "https://api.btcturk.com/api/v2/ticker?pairSymbol=" + coin + "try";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl1);
                    if (response.IsSuccessStatusCode)
                    {
                        ;
                        string responseData = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiDegerleri>(responseData);
                        double lastValue = apiResponse?.Data?.Count > 0 ? apiResponse.Data[0].Last : 0.0;
                        return lastValue;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await UpdateLabelsAsync();
        }
        // veritabanındaki coin verilerinin anlık bilgisini gösterme
        private async Task UpdateLabelsAsync()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
            cmd = new OleDbCommand();
            adp = new OleDbDataAdapter("Select * from " + kullaniciad, con);
            dataSet = new DataSet();
            con.Open();
            adp.Fill(dataSet, kullaniciad);
            con.Close();
            // bağlantı kontrolü
            if (await (UsdtDegeri("btc")) == -1)
            {
                label3.Text = "İNTERNET BAĞLANTI HATASI";
            }
            else
            {
                if (dataSet.Tables[kullaniciad].Rows.Count > 0)
                {
                    label3.Text = "TOPLAM BAKİYELER";
                    // coinlerin usdt değerlerini gösterme
                    DataRow defaultRow = dataSet.Tables[kullaniciad].Rows[0];
                    label9.Text = (Convert.ToDouble(await UsdtDegeri("btc")) * Convert.ToDouble(defaultRow[1])).ToString();
                    label10.Text = (Convert.ToDouble(await UsdtDegeri("eth")) * Convert.ToDouble(defaultRow[2])).ToString();
                    label11.Text = (Convert.ToDouble(await UsdtDegeri("doge")) * Convert.ToDouble(defaultRow[3])).ToString();
                    label12.Text = (Convert.ToDouble(await UsdtDegeri("chz")) * Convert.ToDouble(defaultRow[4])).ToString();
                    label13.Text = (Convert.ToDouble(await UsdtDegeri("trx")) * Convert.ToDouble(defaultRow[5])).ToString();
                    label14.Text = (Convert.ToDouble(await UsdtDegeri("xrp")) * Convert.ToDouble(defaultRow[6])).ToString();

                    // coinlerin try değerlerini gösterme
                    label16.Text = (Convert.ToDouble(await TryDegeri("btc")) * Convert.ToDouble(defaultRow[1])).ToString();
                    label17.Text = (Convert.ToDouble(await TryDegeri("eth")) * Convert.ToDouble(defaultRow[2])).ToString();
                    label18.Text = (Convert.ToDouble(await TryDegeri("doge")) * Convert.ToDouble(defaultRow[3])).ToString();
                    label19.Text = (Convert.ToDouble(await TryDegeri("chz")) * Convert.ToDouble(defaultRow[4])).ToString();
                    label20.Text = (Convert.ToDouble(await TryDegeri("trx")) * Convert.ToDouble(defaultRow[5])).ToString();
                    label21.Text = (Convert.ToDouble(await TryDegeri("xrp")) * Convert.ToDouble(defaultRow[6])).ToString();
                    
                    // coinlerin toplam usdt değerlerini gösterme
                    label24.Text = (Convert.ToDouble(await UsdtDegeri("btc")) * Convert.ToDouble(defaultRow[1]) +
                                    Convert.ToDouble(await UsdtDegeri("eth")) * Convert.ToDouble(defaultRow[2]) +
                                    Convert.ToDouble(await UsdtDegeri("doge")) * Convert.ToDouble(defaultRow[3]) +
                                    Convert.ToDouble(await UsdtDegeri("chz")) * Convert.ToDouble(defaultRow[4]) +
                                    Convert.ToDouble(await UsdtDegeri("trx")) * Convert.ToDouble(defaultRow[5]) +
                                    Convert.ToDouble(await UsdtDegeri("xrp")) * Convert.ToDouble(defaultRow[6])).ToString();

                    //coinlerin toplam try değerlerini gösterme
                    label25.Text = (Convert.ToDouble(await TryDegeri("btc")) * Convert.ToDouble(defaultRow[1]) +
                                    Convert.ToDouble(await TryDegeri("eth")) * Convert.ToDouble(defaultRow[2]) +
                                    Convert.ToDouble(await TryDegeri("doge")) * Convert.ToDouble(defaultRow[3]) +
                                    Convert.ToDouble(await TryDegeri("chz")) * Convert.ToDouble(defaultRow[4]) +
                                    Convert.ToDouble(await TryDegeri("trx")) * Convert.ToDouble(defaultRow[5]) +
                                    Convert.ToDouble(await TryDegeri("xrp")) * Convert.ToDouble(defaultRow[6])).ToString();
                }
            }
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
                timer1.Stop();
        }
        // kullanıcı bilgileri düzenleme ekranı
        private void button4_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(kullaniciad, parola);
            form6.Show();
            this.Close();
        }
    }
}
