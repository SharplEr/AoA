using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using VectorSpace;
using Araneam;
using GenomeNeuralNetwork;
using MyParallel;

namespace WindowsFormsRuner
{
    public partial class Home : Form
    {
        GenomeNetwork network = new GenomeNetwork(0.5, 1000);

        public Home()
        {
            InitializeComponent();
            this.FormClosing += (o, e) =>
            {
                Thread t = new Thread(() => network.Dispose());
                t.SetApartmentState(ApartmentState.MTA);
                t.Start();
                this.Text = "Завершение...";
                this.AddFile.Enabled = false;
                this.StartLearn.Enabled = false;
                this.Check.Enabled = false;
                t.Join();
            };
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Выберите файл для данных обучения";
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Filter = "CSV файлы|*.csv| Все файлы|*";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                testList.Items.Add(dlg.FileName);
            }
        }

        private void StartLearn_Click(object sender, EventArgs e)
        {
            if ((testList.Items == null) || (testList.Items.Count == 0)) return;
            string[] files = new string[testList.Items.Count];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = testList.Items[i].Text;
            }

            if (network.Reload(files))
            {
                new Thread(() => network.EarlyStoppingLearn()).InMTA();
                
                Check.Enabled = true;
            }
        }

        private void Check_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Выберите файл для анализа";
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Filter = "CSV файлы|*.csv| Все файлы|*";
            dlg.Multiselect = false;
            Vector[] result = null;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                new Thread(() => result = network.Calculation(dlg.FileName)).InMTA();

                for (int i = 0; i < result.Length; i++)
                    resultList.Items.Add(result[i].ToString());
            }
        }
    }
}