using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Araneam;
using VectorSpace;
using MyParallel;
using GenomeNeuralNetwork;
using System.Threading;
using System.Diagnostics;
using System.IO;
using IODate;

namespace ConsoleRunner
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            const int n = 1000;
            int[] TestP= new int[n];
            int[] TestN = new int[n];
            int[] TestNP = new int[n];

            int[] ControlP = new int[n];
            int[] ControlN = new int[n];
            int[] ControlNP = new int[n];

            int globalI = 0;
            GenomeNetwork network;
            Vector[] y;
            Vector[] d;
            string name1 = @"..\..\..\Data\data_1.csv";
            string name2 = @"..\..\..\Data\data_2.csv";
            string name1test = @"..\..\..\Data\data_Test1.csv";
            string name2test = @"..\..\..\Data\data_Test2.csv";

            bool ok;
            int p = -1;
            do
            {
                network = new GenomeNetwork(0.1, 3000);
               // GC.Collect();


                if (!new FileInfo(name1).Exists) Console.WriteLine("Первый файл не существует");
                if (!new FileInfo(name2).Exists) Console.WriteLine("Второй файл не существует");

                ok = network.Reload(new string[] { name1, name2 });

                if (!ok)
                {
                    Console.WriteLine("Данные загрузились с ошибкой!");
                    network.Dispose();
                    Console.WriteLine("Все закончилось печально, нажмите <Enter>");
                    Console.ReadLine();
                    return;
                }

                network.EarlyStoppingLearn();

                int countP = 0;
                int countN = 0;
                int countNP = 0;

                y = network.Calculation(name1);

                d = GenomeNetwork.LoadResultDate(new CSVReader(name1));

                if (d.Length != y.Length)
                {
                    Console.WriteLine("Что-то не так");
                    network.Dispose();
                    Console.WriteLine("Все закончилось печально, нажмите <Enter>");
                    Console.ReadLine();
                    return;
                }

                //Тут подразумевается, что ответ представлен в виде скаляра
                for (int i = 0; i < d.Length; i++)
                {
                    if (y[i][0] > 0) countP++;
                    if (y[i][0] == 0) countNP++;
                }

                TestP[globalI] = countP;

                y = network.Calculation(name2);

                d = GenomeNetwork.LoadResultDate(new CSVReader(name2));

                if (d.Length != y.Length)
                {
                    Console.WriteLine("Что-то не так");
                    network.Dispose();
                    Console.WriteLine("Все закончилось печально, нажмите <Enter>");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < d.Length; i++)
                {
                    if (y[i][0] < 0) countN++;
                    if (y[i][0] == 0) countNP++;
                }

                TestN[globalI] = countN;
                TestNP[globalI] = countNP;

                countP = 0;
                countN = 0;
                countNP = 0;

                y = network.Calculation(name1test);
                d = GenomeNetwork.LoadResultDate(new CSVReader(name1test));

                if (d.Length != y.Length)
                {
                    Console.WriteLine("Что-то не так");
                    network.Dispose();
                    Console.WriteLine("Все закончилось печально, нажмите <Enter>");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < d.Length; i++)
                {
                    if (y[i][0] > 0) countP++;
                    if (y[i][0] == 0) countNP++;
                }

                ControlP[globalI] = countP;

                y = network.Calculation(name2test);

                d = GenomeNetwork.LoadResultDate(new CSVReader(name2test));

                if (d.Length != y.Length)
                {
                    Console.WriteLine("Что-то не так");
                    network.Dispose();
                    Console.WriteLine("Все закончилось печально, нажмите <Enter>");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < d.Length; i++)
                {
                    if (y[i][0] < 0) countN++;
                    if (y[i][0] == 0) countNP++;
                }

                ControlN[globalI] = countN;
                ControlNP[globalI] = countNP;

                network.Dispose();
                
                globalI++;
                if (p != 100 * globalI / n)
                {
                    p = 100 * globalI / n;
                    Console.WriteLine("Завершено: {0} %", p);
                }
            } while (globalI < n);
            Console.WriteLine("Все запуски завершены!");

            List<int> LTestP     = new List<int>();
            List<int> LTestN     = new List<int>();
            List<int> LTestNP    = new List<int>();
                                                
            List<int> LControlP  = new List<int>();
            List<int> LControlN  = new List<int>();
            List<int> LControlNP = new List<int>();

            
            for (int i = 0; i < n; i++)
            {
                if ((ControlP[i] > 10) && (ControlN[i] > 19))
                {
                    LTestP.Add(TestP[i]);
                    LTestN.Add(TestN[i]);
                    LTestNP.Add(TestNP[i]);  
                    LControlP.Add(ControlP[i]);
                    LControlN.Add(ControlN[i]);
                    LControlNP.Add(ControlNP[i]);
                }
            }

            Console.WriteLine("Число локальных экстремумов: {0}", n-LTestP.Count);



            double avgTP=0;
            double avgTN=0;
            double avgTNP=0;
            double avgCP=0;
            double avgCN=0;
            double avgCNP=0;
            for (int i = 0; i < LTestP.Count; i++)
            {
                avgTP+=LTestP[i];
                avgTN+=LTestN[i];
                avgTNP+=LTestNP[i];
                avgCP+=LControlP[i];
                avgCN += LControlN[i];
                avgCNP += LControlNP[i];
            }

            avgTP /= LTestP.Count;
            avgTN /= LTestP.Count;
            avgTNP /= LTestP.Count;
            avgCP /= LTestP.Count;
            avgCN /= LTestP.Count;
            avgCNP /= LTestP.Count;

            Console.WriteLine("Среднее число верно опознаных положительных примеров обучающего множества {0} ({1}%)", avgTP, 100*avgTP/81);
            Console.WriteLine("Среднее число верно опознаных отрицательных примеров обучающего множества {0} ({1}%)", avgTN, 100*avgTN/81);
            Console.WriteLine("Среднее число верно опознаных неклассифицированных примеров обучающего множества {0} ({1}%)", avgTNP, 100*avgTNP/162);
            Console.WriteLine("Среднее число верно опознаных положительных примеров контрольного множества {0} ({1}%)",avgCP, 100*avgCP/21);
            Console.WriteLine("Среднее число верно опознаных отрицательных примеров контрольного множества {0} ({1}%)",avgCN, 100*avgCN/39);
            Console.WriteLine("Среднее число верно опознаных неклассифицированных примеров контрольного множества {0} ({1}%)", avgCNP, 100 * avgTP / 60);

            double dTP = 0;
            double dTN = 0;
            double dTNP = 0;
            double dCP = 0;
            double dCN = 0;
            double dCNP = 0;

            for (int i = 0; i < LTestP.Count; i++)
            {
                dTP += (LTestP[i] - avgTP) * (LTestP[i] - avgTP);
                dTN += (LTestN[i] - avgTN) * (LTestN[i] - avgTN);
                dTNP += (LTestNP[i] - avgTNP) * (LTestNP[i] - avgTNP);
                dCP += (LControlP[i] - avgCP) * (LControlP[i] - avgCP);
                dCN += (LControlN[i] - avgCN) * (LControlN[i] - avgCN);
                dCNP += (LControlNP[i] - avgCNP) * (LControlNP[i] - avgCNP);
            }

            dTP = Math.Sqrt(dTP / (LTestP.Count - 1));
            dTN = Math.Sqrt(dTN / (LTestP.Count - 1));
            dTNP = Math.Sqrt(dTNP / (LTestP.Count - 1));
            dCP = Math.Sqrt(dCP / (LTestP.Count - 1));
            dCN = Math.Sqrt(dCN / (LTestP.Count - 1));
            dCNP = Math.Sqrt(dCNP / (LTestP.Count - 1));

            Console.WriteLine("Погрешность положительных примеров обучающего множества {0} ({1}%)",   3*dTP, 300 * dTP / avgTP);
            Console.WriteLine("Погрешность отрицательных примеров обучающего множества {0} ({1}%)", 3 * dTN, 300 * dTN / avgTN);
            Console.WriteLine("Погрешность неклассифицированных примеров обучающего множества {0}", 3 * dTNP);
            Console.WriteLine("Погрешность положительных примеров контрольного множества {0} ({1}%)", 3 * dCP, 300 * dCP / avgCP);
            Console.WriteLine("Погрешность отрицательных примеров контрольного множества {0} ({1}%)", 3 * dCN, 300 * dCN / avgCN);
            Console.WriteLine("Погрешность неклассифицированных примеров контрольного множества {0}", 3 * dCNP);

            double COV = 0.0;
            double dx=0.0;
            double dy = 0.0;
            for (int i = 0; i < LTestP.Count; i++)
            {
                double tx, ty;
                tx = (LTestP[i] + LTestN[i] - avgTP - avgTN) / 162.0;
                ty = LControlP[i] / 21.0 + LControlN[i] / 39.0 - avgCP / 21.0 - avgCN / 39.0;
                COV += tx * ty;
                dx += tx * tx;
                dy += ty * ty;
            }

            double r = COV / Math.Sqrt(dx*dy);

            Console.WriteLine("Коэффициент корреляции между процентом угадывания обучающих примеров и контрольных: {0}", r);

            Console.WriteLine("Все ок, нажмите <Enter>");
            Console.ReadLine();
        }
    }
}