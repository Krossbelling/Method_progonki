﻿using System;
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
            try
            {
                List<TextBox> textBoxes = new List<TextBox> 
                {
                    textBox1, 
                    textBox2, 
                    textBox3, 
                    textBox4, 
                    textBox5, 
                    textBox6,
                    textBox7,
                    textBox8,
                    textBox9,
                    textBox10,
                    textBox11,
                    textBox12,
                    textBox13
                };
                foreach (var s in textBoxes)
                {
                    if (s.Text == "")
                    {
                        s.Text = "0";
                    }
                }

                ParserFunction.addFunction("sin", new SinFunction());
                ParserFunction.addFunction("cos", new CosFunction());
                ParserFunction.addFunction("pi", new PiFunction());
                ParserFunction.addFunction("exp", new ExpFunction());
                ParserFunction.addFunction("pow", new PowFunction());
                ParserFunction.addFunction("abs", new AbsFunction());
                ParserFunction.addFunction("sqrt", new SqrtFunction());
                ParserFunction.addFunction("x", new XFunction());                
                
                double a = textBox5.Text == "0" ? double.Parse(textBox7.Text) : double.Parse(textBox5.Text);
                double b = textBox10.Text == "0" ? double.Parse(textBox12.Text) : double.Parse(textBox10.Text);

                double A = double.Parse(textBox8.Text);
                double B = double.Parse(textBox13.Text);
                double alfa0 = double.Parse(textBox4.Text);
                double alfa1 = double.Parse(textBox6.Text);
                double beta0 = double.Parse(textBox9.Text);
                double beta1 = double.Parse(textBox11.Text);

                double h = 0.05;
                int N = (int)Math.Round((b - a) / h);
                string uravnP = textBox1.Text;
                string uravnQ = textBox2.Text;
                string uravnF = textBox3.Text;



                if (comboBox1.Text == "Метод прогонки")
                {
                    double[] m = new double[N + 1];
                    double[] x = new double[N + 1];
                    double[] n = new double[N + 1];
                    double[] C = new double[N + 1];
                    double[] d = new double[N + 1];
                    double[] y = new double[N + 1];
                    x[0] = a;

                    for (int i = 1; i <= N; i++)
                        x[i] = x[0] + i * h;

                    for (int i = 0; i <= N; i++)
                        m[i] = -2 + h * Parser.process(x[i],uravnP);                    

                    for (int i = 0; i <= N; i++)
                        n[i] = 1 - h * Parser.process(x[i], uravnP) + Parser.process(x[i], uravnQ) * Math.Pow(h, 2);

                    C[0] = (alfa1 - alfa0 * h) / (m[0] * (alfa1 - alfa0 * h) + n[0] * alfa1);
                    for (int i = 1; i < N - 1; i++)
                        C[i] = 1 / (m[i] - n[i] * C[i - 1]);                   

                    d[0] = (n[0] * A * h) / (alfa1 - alfa0 * h) + Parser.process(x[0], uravnF)  * h;
                    for (int i = 1; i < N - 1; i++)
                        d[i] = Parser.process(x[i], uravnF) * Math.Pow(h, 2) - n[i] * C[i - 1] * d[i - 1];                    

                    y[N] = (beta1 * C[N - 2] * d[N - 2] + B * h) / (beta1 * (1 + C[N - 2]) + beta0 * h);
                    for (int i = N - 1; i > 0; i--)
                        y[i] = C[i - 1] * (d[i - 1] - y[i + 1]);
                    y[0] = (alfa1 * y[1] - A * h) / (alfa1 - alfa0 * h);

                    dataGridView1.Rows.Clear();
                    for (int i = 0; i < N + 1; i++)
                    {
                        dataGridView1.Rows.Add(i, x[i], y[i]);
                    }
                }
                if (comboBox1.Text == "Метод прогонки с центральной разностью")
                {
                    double[] m = new double[N + 1];
                    double[] x = new double[N + 1];
                    double[] n = new double[N + 1];
                    double[] C = new double[N + 1];
                    double[] d = new double[N + 1];
                    double[] y = new double[N + 1];
                    x[0] = a;

                    for (int i = 1; i <= N; i++)
                        x[i] = x[0] + i * h;

                    for (int i = 0; i <= N; i++)
                        m[i] = (2 * Parser.process(x[i], uravnQ) * Math.Pow(h, 2) - 4) / (2 + h * Parser.process(x[i], uravnP));                    

                    for (int i = 0; i <= N; i++)
                        n[i] = (2 - h * Parser.process(x[i], uravnP)) / (2 + h * Parser.process(x[i], uravnP));

                    C[1] = (alfa1 - alfa0 * h) / (m[1] * (alfa1 - alfa0 * h) + n[1] * alfa1);
                    for (int i = 2; i <= N; i++)
                        C[i] = 1 / (m[i] - n[i] * C[i - 1]);                    

                    d[1] = (2 * Parser.process(x[1], uravnF) * Math.Pow(h, 2)) / (2 + h * Parser.process(x[1], uravnP)) + (n[1] * A * h) / (alfa1 - alfa0 * h);
                    for (int i = 2; i <= N; i++)
                        d[i] = ((2 * Parser.process(x[i], uravnF) * Math.Pow(h, 2)) / (2 + h * Parser.process(x[i], uravnP)) - n[i] * C[i - 1] * d[i - 1]);                   

                    y[N] = ((2 * B * h) - beta1 * (d[N] - C[N - 1] * d[N - 1]))/ ((2 * beta0 * h) + beta1 * (C[N - 1] - (1 / C[N])));
                    for (int i = N - 1; i > 0; i--)
                        y[i] = C[i] * (d[i] - y[i + 1]);
                    y[0] = (A * h - alfa1 * y[1]) / (alfa0 * h - alfa1);

                    dataGridView1.Rows.Clear();
                    for (int i = 0; i < N + 1; i++)
                    {
                        dataGridView1.Rows.Add(i, x[i], y[i]);
                    }
                }

            }
            catch (Exception exept)
            {
                MessageBox.Show("Решить не удалось" + ".\n " + exept.Message + "\n " + "Проверьте ввод данных:" + "\n " + "дифференциальное уравнение, a, b, x0, y0, N, eps.");
            }
        }

        
    }
}
