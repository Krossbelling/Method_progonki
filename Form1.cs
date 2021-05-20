using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Method_pro
{
    public partial class Form1 : Form
    {
        Button but;
        bool progonkiAll = true;
        bool progonkiScentrAll = true;
        public Form1()
        {
            InitializeComponent();
            comboBox1.Text = "Метод прогонки";
        }                
        private void button1_Click(object sender, EventArgs e)
        {
            ColorButton(sender);
            
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
            textBox14.Text = "0,05";
        }

        private void ColorButton(object sender)
        {
            if (but != null)
                but.BackColor = Color.DarkGoldenrod;
            but = (Button)sender;
            but.BackColor = Color.Gold;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorButton(sender);
            
            textBox1.Text = "-2*x";
            textBox2.Text = "-2";
            textBox3.Text = "-4*x";
            textBox4.Text = "1";
            textBox5.Text = "0";
            textBox6.Text = "-1";
            textBox7.Text = "0";
            textBox8.Text = "0";
            textBox9.Text = "1";
            textBox10.Text = "1";
            textBox11.Text = "0";
            textBox12.Text = "0";
            textBox13.Text = "3,718";
            textBox14.Text = "0,1";

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
                        s.Text = "0";
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

                double h = double.Parse(textBox14.Text);
                int N = (int)Math.Round((b - a) / h);
                string uravnP = textBox1.Text;
                string uravnQ = textBox2.Text;
                string uravnF = textBox3.Text;

                double[] m = new double[N + 1];
                double[] x = new double[N + 1];
                double[] n = new double[N + 1];
                double[] C = new double[N + 1];
                double[] d = new double[N + 1];
                double[] y = new double[N + 1];
                x[0] = a;
                for (int i = 1; i <= N; i++)
                    x[i] = x[0] + i * h;

                if (comboBox1.Text == "Метод прогонки")
                {
                    for (int i = 0; i <= N; i++)
                    {
                        m[i] = -2 + h * Parser.process(x[i], uravnP);
                        n[i] = 1 - h * Parser.process(x[i], uravnP) + Parser.process(x[i], uravnQ) * Math.Pow(h, 2);
                    }

                    C[0] = (alfa1 - alfa0 * h) / (m[0] * (alfa1 - alfa0 * h) + n[0] * alfa1);
                    d[0] = (n[0] * A * h) / (alfa1 - alfa0 * h) + Parser.process(x[0], uravnF) * Math.Pow(h, 2);
                    for (int i = 1; i < N - 1; i++)
                    {
                        C[i] = 1 / (m[i] - n[i] * C[i - 1]);
                        d[i] = Parser.process(x[i], uravnF) * Math.Pow(h, 2) - n[i] * C[i - 1] * d[i - 1];
                    }

                    y[N] = (beta1 * C[N - 2] * d[N - 2] + B * h) / (beta1 * (1 + C[N - 2]) + beta0 * h);
                    for (int i = N - 1; i > 0; i--)
                        y[i] = C[i - 1] * (d[i - 1] - y[i + 1]);
                    y[0] = (alfa1 * y[1] - A * h) / (alfa1 - alfa0 * h);

                    Color c1 = Color.FromArgb(255, 0, 0);
                    Color c2 = Color.FromArgb(128, 0, 255);
                    if (checkBox1.Checked == true && progonkiAll == true)
                    {
                        OutputAll(N, m, x, n, C, d, y, "Метод прогонки", c1);
                        progonkiAll = false;
                    }
                       
                    else if (checkBox1.Checked == false)
                        OutputOne(N, m, x, n, C, d, y, "Метод прогонки",c2,0);
                }
                if (comboBox1.Text == "Метод прогонки с центральной разностью")
                {
                    for (int i = 0; i <= N; i++)
                    {
                        m[i] = (2 * Parser.process(x[i], uravnQ) * Math.Pow(h, 2) - 4) / (2 + h * Parser.process(x[i], uravnP));
                        n[i] = (2 - h * Parser.process(x[i], uravnP)) / (2 + h * Parser.process(x[i], uravnP));
                    }

                    C[1] = (alfa1 - alfa0 * h) / (m[1] * (alfa1 - alfa0 * h) + n[1] * alfa1);
                    d[1] = (2 * Parser.process(x[1], uravnF) * Math.Pow(h, 2)) / (2 + h * Parser.process(x[1], uravnP)) + (n[1] * A * h) / (alfa1 - alfa0 * h);
                    for (int i = 2; i <= N; i++)
                    {
                        C[i] = 1 / (m[i] - n[i] * C[i - 1]);
                        d[i] = ((2 * Parser.process(x[i], uravnF) * Math.Pow(h, 2)) / (2 + h * Parser.process(x[i], uravnP)) - n[i] * C[i - 1] * d[i - 1]);
                    }

                    y[N] = ((2 * B * h) - beta1 * (d[N] - C[N - 1] * d[N - 1])) / ((2 * beta0 * h) + beta1 * (C[N - 1] - (1 / C[N])));
                    for (int i = N - 1; i > 0; i--)
                        y[i] = C[i] * (d[i] - y[i + 1]);
                    y[0] = (A * h - alfa1 * y[1]) / (alfa0 * h - alfa1);

                    Color c1 = Color.FromArgb(0, 0, 255);
                    Color c2 = Color.FromArgb(128, 0, 255);
                    if (checkBox1.Checked == true && progonkiScentrAll == true)
                    {
                        OutputAll(N, m, x, n, C, d, y, "Метод прогонки с центральной разностью", c1);
                        progonkiScentrAll = false;
                    }

                    else if (checkBox1.Checked == false)
                        OutputOne(N, m, x, n, C, d, y, "Метод прогонки с центральной разностью", c2,1);


                }
                if (comboBox1.Text == "") // пока что не работает  "Метод конечных разностей")
                {
                    double s = 0;
                    double[] strichY = new double[N];
                    double[] strich2Y = new double[N];
                    double[] gausY = new double[N];
                    double[,] gausA = new double[N, N];
                    double[] gausB = new double[N];
                    double[] gausX = new double[N];


                    for (int i = 0; i < N; i++)
                    {
                        strichY[i] = (y[i + 1] - y[i]) / h;
                        strich2Y[i] = (y[i + 2] - 2 * y[i + 1] + y[i]) / Math.Pow(h, 2);

                        gausY[i] = strichY[i] * y[i];


                        //Parser.process(x[i], uravnF) = (y[i - 1] + 2 * y[i] + y[i - 1]) / Math.Pow(h, 2) + Parser.process(x[i], uravnP) * (y[i + 1] - y[i - 1]) / 2 * h + Parser.process(x[i], uravnQ) * y[i];
                        //alfa0*y[0]+alfa1 * (y[1] - y[0]) / h = A;
                        // beta0 * y[0]+beta1 * (y[1] - y[0]) / h = B;



                    }


                    for (int i = 0; i < N; i++)
                        gausX[i] = 0;

                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                        {
                            // gausA[i, j] = double.Parse(x[i]);
                        }

                    for (int i = 0; i < N; i++)
                        gausB[i] = double.Parse(Console.ReadLine());

                    for (int k = 0; k < N - 1; k++)
                    {
                        for (int i = k + 1; i < N; i++)
                        {
                            for (int j = k + 1; j < N; j++)
                            {
                                gausA[i, j] = gausA[i, j] - gausA[k, j] * (gausA[i, k] / gausA[k, k]);
                            }
                            gausB[i] = gausB[i] - gausB[k] * gausA[i, k] / gausA[k, k];
                        }
                    }
                    for (int k = N - 1; k >= 0; k--)
                    {
                        s = 0;
                        for (int j = k + 1; j < N; j++)
                            s = s + gausA[k, j] * gausX[j];
                        gausX[k] = (gausB[k] - s) / gausA[k, k];
                    }
                    for (int i = 0; i < gausX.Length; i++)
                    {


                    }

                }
               

            }
            catch (Exception exept)
            {
                MessageBox.Show("Решить не удалось" + ".\n " + exept.Message + "\n " + "Проверьте ввод данных:" + "\n ");
            }
        }

        private void OutputAll(int N, double[] m, double[] x, double[] n, double[] C, double[] d, double[] y, string MethodName, Color color)
        {
            
            dataGridView1.Rows.Clear();
            //ResultChart.Series.Clear();
            //ResultChart.Legends.Clear();
            ResultChart.Titles[0].Text = "Численные методы";
            Series series = new Series()
            {               
                Name = MethodName,
                ChartType = SeriesChartType.Line,
                Color = color
            };                   

            for (int i = 0; i < N + 1; i++)
            {
                dataGridView1.Rows.Add(i, x[i], m[i], n[i], C[i], d[i], y[i]);
                series.Points.AddXY(x[i], y[i]);
            }
            ResultChart.Legends.Add(MethodName);
            ResultChart.Series.Add(series);            
            
        }
        private void OutputOne(int N, double[] m, double[] x, double[] n, double[] C, double[] d, double[] y, string MethodName, Color color, byte progonki)
        {

            dataGridView1.Rows.Clear();
            ResultChart.Series.Clear();
            ResultChart.Legends.Clear();
            Series series = new Series()
            {
                Name = MethodName,
                ChartType = SeriesChartType.Line,
                Color = color
            };
            ResultChart.Titles[0].Text = MethodName;           

            for (int i = 0; i < N + 1; i++)
            {
                dataGridView1.Rows.Add(i, x[i], m[i], n[i], C[i], d[i], y[i]);
                series.Points.AddXY(x[i], y[i]);
            }
            ResultChart.Legends.Add(MethodName);
            ResultChart.Series.Add(series);
            progonkiScentrAll = true;
            progonkiAll = true;
            if (progonki == 0)
                progonkiAll = false;
            if (progonki==1)
                progonkiScentrAll = false;
            
            
        }
    }
}
