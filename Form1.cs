using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;



namespace MQTT_Test
{
    public partial class Form1 : Form
    {
        public class HuePod
        {
            public string name { get; set; }
            public string function { get; set; }
            public IList<int> color { get; set; }
        }

        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            label1.Text = "Rot anteil: ";
            label2.Text = "Grün anteil: ";
            label3.Text = "Blau anteil: ";
            label4.Text = "Empfangen vom Sub: ";
            label5.Text = "Server Addresse: ";
            label6.Text = "Port: ";
            label7.Text = "Subscribe: ";
            label8.Text = "Publish: ";
            label9.Text = "Text zum Senden: ";
            label10.Text = "Funktion"; 

            button1.Text = "Verbinden";
            button2.Text = "Schliessen";
            button3.Text = "Setup Color";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text == "" && textBox4.Text == "")
                {
                    MessageBox.Show(this, "Die Felder sind noch Leer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var client = new MqttClient(textBox1.Text.ToString(), int.Parse(textBox2.Text), false, MqttSslProtocols.None, null, null);
                    client.ProtocolVersion = MqttProtocolVersion.Version_3_1;

                    client.MqttMsgPublishReceived += client_receivedMessage;

                    byte code = client.Connect(Guid.NewGuid().ToString(), " ", " ");

                    if (code == 0)
                    {
                        MessageBox.Show(this, "Konnte sich verbinden", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        client.Subscribe(new string[] { textBox3.Text.ToString() }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                        client.Publish(textBox4.Text.ToString(), Encoding.UTF8.GetBytes(textBox5.Text.ToString()));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Irgend ein Fehler ist aufgetreten", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        static void client_receivedMessage(object sender, MqttMsgPublishEventArgs e)
        {
            // Handle received messages
            var message = System.Text.Encoding.Default.GetString(e.Message);

            //MessageBox.Show("Message received: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                HuePod pod = new HuePod
                {
                    name = "HuePod",
                    function = textBox7.Text.ToString(),
                    color = new List<int>
                    {
                        trackBar2.Value,
                        trackBar3.Value,
                        trackBar1.Value
                    }
                };

                string jsoon = JsonConvert.SerializeObject(pod, Formatting.Indented);
                textBox5.Text = jsoon;
                //MessageBox.Show("Json String: " + jsoon, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Irgend ein Fehler ist aufgetreten", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
