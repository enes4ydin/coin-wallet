using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Reflection.Emit;

// Enes AYDIN 20010207042
namespace coin
{
    public partial class Form5 : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader dr;
        public Form5()
        {
            InitializeComponent();
        }
        // yeni kullanıcı için tablo ve verileri oluşturma
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" &&
                textBox5.Text != "" && textBox6.Text != "" && textBox7.Text != "" && textBox8.Text != "")
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
                con.Open();
                bool kullaniciVarMi = KullaniciVarMi(textBox1.Text, con);
                // aynı isime sahip kullanıcı kontrolü
                if (kullaniciVarMi)
                {
                    MessageBox.Show("Aynı isime sahip kullanıcı var, başka isim deneyiniz");
                }
                // yok ise yeni kullanıcı oluşturma
                else
                {
                    string tableName = textBox1.Text.ToLower();
                    string parola = textBox2.Text;
                    string columns = "kullanici VARCHAR(255), btc INT, eth INT, doge INT, chz INT, trx INT, xrp INT";

                    string createTableQuery = $"CREATE TABLE {tableName} ({columns})";
                    using (OleDbCommand command = new OleDbCommand(createTableQuery, con))
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Yeni kayıt oluşturuldu");
                    }
                    string insertQuery = "INSERT INTO " + tableName + " (kullanici, btc, eth, doge, chz, trx, xrp) VALUES (?, ?, ?, ?, ?, ?, ?)";
                    cmd = new OleDbCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("kullanici", tableName);
                    cmd.Parameters.AddWithValue("btc", Convert.ToInt32(textBox3.Text));
                    cmd.Parameters.AddWithValue("eth", Convert.ToInt32(textBox4.Text));
                    cmd.Parameters.AddWithValue("doge", Convert.ToInt32(textBox5.Text));
                    cmd.Parameters.AddWithValue("chz", Convert.ToInt32(textBox6.Text));
                    cmd.Parameters.AddWithValue("trx", Convert.ToInt32(textBox7.Text));
                    cmd.Parameters.AddWithValue("xrp", Convert.ToInt32(textBox8.Text));
                    cmd.ExecuteNonQuery();

                    string insertQuery2 = "INSERT INTO Users (User_Name, Parola) VALUES (?, ?)";
                    cmd = new OleDbCommand(insertQuery2, con);
                    cmd.Parameters.AddWithValue("User_Name", textBox1.Text);
                    cmd.Parameters.AddWithValue("Parola", textBox2.Text);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    this.Close();
                }
            }
            // boş ifade kontrolü
            else
            {
                MessageBox.Show("Boş ifade bırakmayınız");
            }
        }
        // aynı isime sahip olan tablonun sorgusu
        public bool KullaniciVarMi(string yeniKullaniciAdi, OleDbConnection con)
        {
        using (OleDbCommand cmd = new OleDbCommand($"SELECT COUNT(*) FROM Users WHERE User_Name = @YeniKullaniciAdi", con))
        {
            cmd.Parameters.AddWithValue("@YeniKullaniciAdi", textBox1.Text);
            int kullaniciSayisi = (int)cmd.ExecuteScalar();
            return kullaniciSayisi > 0;
            }
        }
    }
}
