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
using Newtonsoft.Json;

namespace server__sql
{
    public partial class Form1 : Form
    {
        SqlConnection Connection = new SqlConnection();
        string ConnectionAddress = "Server=.;Database=Registers;User Id=sa;Password=123456;connection timeout=30;";

        public Form1()
        {
            InitializeComponent();
        }

        SimpleTCP.SimpleTcpServer server;

        public class Alarms
        {
            public string serialnumber { get; set; }
            public int P1 { get; set; }
            public int P2 { get; set; }
            public int P3 { get; set; }
            public int P4 { get; set; }
            public int P5 { get; set; }
            public int P6 { get; set; }
            public int P7 { get; set; }
            public int P8 { get; set; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTCP.SimpleTcpServer();
            server.Delimiter = 0x13;//enter
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
            
        }
       
        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Clear();
                txtStatus.Text += e.MessageString;
                e.ReplyLine(string.Format("You said: {0}", e.MessageString));
                string jsontext = txtStatus.Text;
                Alarms alarms = JsonConvert.DeserializeObject<Alarms>(jsontext);
                //textBox1.Text = (alarmlar.P1).ToString();
                getvalue((alarms.P1).ToString(), (alarms.P2).ToString(), (alarms.P3).ToString(), (alarms.P4).ToString(), (alarms.P5).ToString(), (alarms.P6).ToString(), (alarms.P7).ToString(), (alarms.P8).ToString(), (alarms.serialnumber).ToString());
            });
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            //Start server host
            progressBar1.Value = 100;
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
            server.Start(ip, Convert.ToInt32(txtPort.Text));
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
                server.Stop();
            progressBar1.Value = 0;

        }
        private void getvalue(string P1, string P2, string P3, string P4, string P5, string P6, string P7, string P8, string serialnumber)
        {
            Connection.ConnectionString = ConnectionAddress;
            Connection.Open();
            string register = "INSERT INTO stateofports(Date,serialnumber,P1,P2,P3,P4,P5,P6,P7,P8) VALUES (@Date,@serialnumber,@P1,@P2,@P3,@P4,@P5,@P6,@P7,@P8)";
            SqlCommand komut = new SqlCommand(register, Connection);
            komut.Parameters.AddWithValue("@Date", DateTime.Now.ToString("MM/dd/yyyy HH:mm"));
            komut.Parameters.AddWithValue("serialnumber", serialnumber);
            komut.Parameters.AddWithValue("@P1", P1);
            komut.Parameters.AddWithValue("@P2", P2);
            komut.Parameters.AddWithValue("@P3", P3);
            komut.Parameters.AddWithValue("@P4", P4);
            komut.Parameters.AddWithValue("@P5", P5);
            komut.Parameters.AddWithValue("@P6", P6);
            komut.Parameters.AddWithValue("@P7", P7);
            komut.Parameters.AddWithValue("@P8", P8);
            komut.ExecuteNonQuery();
            Connection.Close();
        }
    }
}
