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

// Enes AYDIN 20010207042
namespace coin
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader dr;
        private void button1_Click(object sender, EventArgs e)
        {
            // kullanıcı sorgusu
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string kullaniciad = textBox1.Text.ToLower();
                string parola = textBox2.Text;
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=VeriTabani.accdb");
                cmd = new OleDbCommand();
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM Users where User_Name='" + textBox1.Text + "' AND Parola='" + textBox2.Text + "'";
                dr = cmd.ExecuteReader();
                // başarılı giriş
                if (dr.Read())
                {
                    Form4 form4 = new Form4(kullaniciad, parola);
                    form4.Show();
                    con.Close();
                    this.Close();
                }
                // hatalı giriş
                else
                {
                    MessageBox.Show("Kullanıcı adı ya da şifre yanlış");
                }
                con.Close();
            }
            else
            {
                MessageBox.Show("Boş ifade bırakmayınız");
            }
        }
        // yeni kayıt ekranı
        private void button2_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }
    }
}
