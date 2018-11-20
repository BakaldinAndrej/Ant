using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ant
{
    public partial class Form1 : Form
    {
        const int N = 8;
        const int Q = 100;
        const double a = 0.5;
        const double b = 5.0;
        const double pheromonInit = 1.0 / N;
        const int antCount = 8;
        Random rand;
        int[] mas = new int[N];
        double[,] niff = new double[N, N];
        int[,] tabu = new int[N, N];

        public Form1()
        {
            InitializeComponent();
            dgv.RowCount = dgv.ColumnCount = N;
            for (int i = 0; i < N; i++)
            {
                dgv.Columns[i].HeaderText = (i + 1).ToString();
                for (int j = 0; j < N; j++)
                    dgv[i, j].Value = pheromonInit;
            }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    niff[i, j] = pheromonInit;
                }
                tabu[i, i] = 1;
                mas[i] = i;
            }

            rand = new Random();
        }

        private int CalcErrors()
        {
            int e = 0;
            Point[] d = { new Point(-1, -1), new Point(1, 1), new Point(-1, 1), new Point(1, -1) };
            int tx = 0;
            int ty = 0;

            for (int x = 0; x < N; x++)
            {
                for (int j = 0; j < N; j++)
                    if ((mas[x] == mas[j]) && (x != j))
                        e++;

                for (int i = 0; i < 4; i++)
                {
                    tx = x + d[i].X;
                    ty = mas[x] + d[i].Y;

                    while ((tx >= 0) && (tx < N) && (ty >= 0) && (ty < N))
                    {
                        if (mas[tx] == ty) e++;

                        tx += d[i].X;
                        ty += d[i].Y;
                    }
                }
            }

            return e;
        }

        double antProduct(int i, int j)
        {
            return ((Math.Pow(niff[i, j], a) * Math.Pow((1.0 / CalcErrors()), b)));
        }

        private int SelectNaxtCell(int ant)
        {
            double sum = 0;
            for (int j = 0; j < N; j++)
            {
                if (tabu[ant, j] == 0) sum += niff[ant, j];
            }

            double p = 0;
            double[] pp = new double[N];
            int answ=0;
            for (int j = 0; j < N; j++)
            {
                if (tabu[ant, j] == 0)
                {
                    pp[j] = antProduct(ant, j) / sum;
                    p += pp[j];
                }
            }
            double temp = rand.NextDouble() * p;

            p = 0;
            for (int j=0;j<N;j++)
            {
                if ((temp > p)&&(tabu[ant,j] == 0))
                    answ = j;
                p += pp[j];
            }
            return answ;
        }

        private void AntCalc()
        {
            for (int step = 0; step < 100; step++)
            {
                for (int i = 0; i < N; i++)
                {
                    mas[i] = SelectNaxtCell(i);
                    tabu[i, mas[i]] = 1;
                    int temp = 0;
                    for (int j = 0; j < N; j++)
                        if (tabu[i, j] == 1)
                            temp++;
                    if (temp == N)
                    {
                        for (int j = 0; j < N; j++)
                            tabu[i, j] = 0;
                        tabu[i, mas[i]] = 1;
                    }

                    for (int j = 0; j < N; j++)
                    {
                        niff[i, j] = niff[i, j] * 0.5 + Q / CalcErrors();
                    }
                }
            }
        }

        private void _btnStart_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            AntCalc();
            Cursor.Current = Cursors.Default;

            _tbAnswer.Text = "";
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    dgv[i, j].Value = niff[i, j];
                }
                _tbAnswer.Text += mas[i];
            }

            _tbError.Text = CalcErrors().ToString();
        }
    }
}
