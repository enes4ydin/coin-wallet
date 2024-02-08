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
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

// Enes AYDIN 20010207042
namespace coin
{
    public partial class Form6 : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader dr;
        OleDbDataAdapter adp;
        DataSet dataSet;
        private string kullaniciad;
        private string parola;
        public Form6(string kullaniciad, string parola)
        {
            InitializeComponent();
            this.kullaniciad = kullaniciad;
            this.parola = parola;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Kullanıcıyı ve Verilerini silmek istiyor musunuz?", "Uyarı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string tableName = "" + kullaniciad + "";
                string connectionString = "Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    // kullanıcının adını, parolasını ve tablosunu silme
                    connection.Open();
                    string deleteTableQuery = "DROP TABLE " + tableName;
                    using (OleDbCommand command = new OleDbCommand(deleteTableQuery, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
                            cmd = new OleDbCommand();
                            con.Open();
                            cmd.Connection = con;
                            cmd.CommandText = "Delete FROM Users where User_Name='" + kullaniciad + "'";
                            cmd.ExecuteNonQuery();
                            connection.Close();
                            textBox1.Text = "";
                            textBox2.Text = "";
                            MessageBox.Show("Tablo başarıyla silindi.");

                            Form3 form3 = new Form3();
                            form3.Show();
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("İşlem iptal edildi");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // kullanıcı adını güncelleme
            if (textBox1.Text != "")
            {
                string connectionString = "Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb";

                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    bool kullaniciVarMi = KullaniciVarMi(textBox1.Text, con);

                    if (textBox1.Text == kullaniciad)
                    {
                        MessageBox.Show("Aynı isimi kullanmayınız");
                    }
                    else if (kullaniciVarMi)
                    {
                        // aynı isme sahip kullanıcı var
                        MessageBox.Show("Aynı isime sahip kullanıcı var, başka isim deneyiniz");
                    }
                    else
                    {
                        // kullanıcı bilgilerini güncelleme sorgusu
                        using (OleDbCommand updateCmd = new OleDbCommand($"UPDATE Users SET User_Name = @YeniKullaniciAdi WHERE User_Name = @EskiKullaniciAdi", con))
                        {
                            updateCmd.Parameters.AddWithValue("@YeniKullaniciAdi", textBox1.Text);
                            updateCmd.Parameters.AddWithValue("@EskiKullaniciAdi", kullaniciad);
                            updateCmd.ExecuteNonQuery();
                        }
                        using (OleDbCommand updateCmd1 = new OleDbCommand($"UPDATE {kullaniciad} SET kullanici = @YeniKullaniciAdi1 WHERE kullanici = @EskiKullaniciAdi1", con))
                        {
                            updateCmd1.Parameters.AddWithValue("@YeniKullaniciAdi1", textBox1.Text);
                            updateCmd1.Parameters.AddWithValue("@EskiKullaniciAdi1", kullaniciad);
                            updateCmd1.ExecuteNonQuery();
                        }
                        // tablo adını değiştirme sorgusu
                        using (OleDbCommand backupCmd = new OleDbCommand($"SELECT * INTO {kullaniciad}_Backup FROM {kullaniciad}", con))
                        {
                            backupCmd.ExecuteNonQuery();
                        }

                        // yeni adla yeni tablo oluştur
                        using (OleDbCommand createCmd = new OleDbCommand($"SELECT * INTO {textBox1.Text} FROM {kullaniciad} WHERE 1=0", con))
                        {
                            createCmd.ExecuteNonQuery();
                        }

                        // eski tablonun içeriğini yeni tabloya kopyala
                        using (OleDbCommand copyCmd = new OleDbCommand($"INSERT INTO {textBox1.Text} SELECT * FROM {kullaniciad}", con))
                        {
                            copyCmd.ExecuteNonQuery();
                        }

                        // eski tabloyu silme
                        using (OleDbCommand dropCmd = new OleDbCommand($"DROP TABLE {kullaniciad}", con))
                        {
                            dropCmd.ExecuteNonQuery();
                        }
                        label1.Text = "Kullancı: " + textBox1.Text;
                        kullaniciad = textBox1.Text;
                        con.Close();
                        MessageBox.Show("Kullanıcı adı " + textBox1.Text + " olarak başarıyla değiştirildi");
                        textBox1.Text = "";
                    }
                }
            }
            else
            {
                MessageBox.Show("Bir kullanıcı adı giriniz");
            }
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            label1.Text = "Kullancı: " + kullaniciad;
        }
        public bool KullaniciVarMi(string yeniKullaniciAdi, OleDbConnection con)
        {
            using (OleDbCommand cmd = new OleDbCommand($"SELECT COUNT(*) FROM Users WHERE User_Name = @YeniKullaniciAdi", con))
            {
                cmd.Parameters.AddWithValue("@YeniKullaniciAdi", textBox1.Text);
                int kullaniciSayisi = (int)cmd.ExecuteScalar();
                return kullaniciSayisi > 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // şifre bilgilerini güncelleme
            if (textBox2.Text != "")
            {
                string connectionString = "Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb";

                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    // kullanıcı bilgilerini güncelleme sorgusu
                    using (OleDbCommand updateCmd = new OleDbCommand($"UPDATE Users SET Parola = @YeniParola WHERE User_Name = @EskiKullaniciAdi", con))
                    {
                        updateCmd.Parameters.AddWithValue("@YeniParola", textBox2.Text);
                        updateCmd.Parameters.AddWithValue("@EskiKullaniciAdi", kullaniciad);
                        updateCmd.ExecuteNonQuery();
                    }
                    textBox2.Text = "";
                    con.Close();
                    MessageBox.Show(kullaniciad + " kullanıcısının parolası başarıyla değiştirildi");
                }
            }
            else
            {
                MessageBox.Show("Bir parola giriniz");
            }
        }
    }
}
