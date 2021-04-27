using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Method_pro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Text = "Метод прогонки";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "x";
            textBox2.Text = "1";
            textBox3.Text = "x+1";
            textBox4.Text = "1";
            textBox5.Text = "0,5";
            textBox6.Text = "2";
            textBox7.Text = "0,5";
            textBox8.Text = "1";
            textBox9.Text = "1";
            textBox10.Text = "0,8";
            textBox11.Text = "0";
            textBox12.Text = "0";
            textBox13.Text = "1,2";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Метод прогонки")
            {
                
            }
        }
    }
}
