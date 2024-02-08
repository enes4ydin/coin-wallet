using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Net.Http;

// Enes AYDIN 20010207042
namespace coin
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public class ApiDegerleri
        {
            public List<CoinVerileri> Data { get; set; }
        }

        public class CoinVerileri
        {
            public double Last { get; set; }
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

        async private void timer1_Tick(object sender, EventArgs e)
        {
            // internet kontrolü
            if (await(UsdtDegeri("btc")) == -1)
            {
                label3.Text = "İNTERNET BAĞLANTI HATASI";
            }
            else
            {
                // coinlerin usdt değerlerini gösterme
                label3.Text = "GÜNCEL COİN PİYASASI";
                label9.Text = Convert.ToDouble(await UsdtDegeri("btc")).ToString();
                label10.Text = Convert.ToDouble(await UsdtDegeri("eth")).ToString();
                label11.Text = Convert.ToDouble(await UsdtDegeri("doge")).ToString();
                label12.Text = Convert.ToDouble(await UsdtDegeri("chz")).ToString();
                label13.Text = Convert.ToDouble(await UsdtDegeri("trx")).ToString();
                label14.Text = Convert.ToDouble(await UsdtDegeri("xrp")).ToString();

                // coinlerin try değerlerini gösterme
                label25.Text = Convert.ToDouble(await TryDegeri("usdt")).ToString();
                label16.Text = Convert.ToDouble(await TryDegeri("btc")).ToString();
                label17.Text = Convert.ToDouble(await TryDegeri("eth")).ToString();
                label18.Text = Convert.ToDouble(await TryDegeri("doge")).ToString();
                label19.Text = Convert.ToDouble(await TryDegeri("chz")).ToString();
                label20.Text = Convert.ToDouble(await TryDegeri("trx")).ToString();
                label21.Text = Convert.ToDouble(await TryDegeri("xrp")).ToString();
            }
        }
    }
}
