using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public ListView listView;
        public Button cmpButton;

        public Form2(ListView listView1, Button cmpButton1)
        {
            listView = listView1;
            cmpButton = cmpButton1;
            InitializeComponent();
            this.Text = "Adding activities";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Action temp = new Action();
                temp.name = textBox1.Text;
                temp.previous = int.Parse(textBox2.Text);
                temp.next = int.Parse(textBox3.Text);
                temp.time = double.Parse(textBox4.Text);
                ListViewItem item = new ListViewItem(temp.name);
                item.SubItems.Add(temp.previous.ToString());
                item.SubItems.Add(temp.next.ToString());
                item.SubItems.Add(temp.time.ToString());
                listView.Items.Add(item);
                cmpButton.Enabled = true;
                this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("DATA ERROR");
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
