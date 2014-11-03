using System;
using System.IO;
using IOData;
using VectorSpace;
using AoA;
using GenomeNeuralNetwork;
using Metaheuristics;
using System.Collections.Generic;
using System.Diagnostics;

namespace AoARun
{
    class Program
    {
        static void Main(string[] args)
        {
            
            FullData data = new FullData(new string[] { @"..\..\..\Data\data_1.csv", @"..\..\..\Data\data_2.csv" },
                GenomeNetwork.TestTags,
                GenomeNetwork.ResultTags[0],
                GenomeNetwork.FenTags,
                GenomeNetwork.ToDouble);            
            
            /*
            FullData data = new FullData(new string[] { @"..\..\..\Data\Iris.csv" },
                new string[] { "Длинач", "Ширинач", "Длинал", "Ширинал"},
                "Вид",
                new string[] { "Длинач", "Ширинач", "Длинал", "Ширинал" },
                GenomeNetwork.ToDouble);
              */
            //21,30,11-0,641+0,005
            //35-24-9-0,664-лучшее, при 0.1, 5500
            //
            
            const int mmm = 100;

            var tupleSigment = DataManager.getShuffleFrom(data, mmm, 0.25, new Random(271828314));
            
            //0,21-500-5-24-9
            
            //Experiments experiment = new Experiments(() => new AGN(0.01, 2800, 13, 65, 20), mmm);
            //1-2-10-6
            //1-6-1-13-2,05
            
            Experiments experiment = new Experiments(() => new AGRNN(4,8, 7, 3, 2.05), mmm);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CVlog v = experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}%", x * 100); }, tupleSigment.Item1, tupleSigment.Item2);
            sw.Stop();

            if (v.Save(new StreamWriter(@"log.txt", false))) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");
            /*
            if (experiment.WriteLog(@"log.txt")) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");
            */
            Console.WriteLine("Время: {0} (мс/обучение)", (double)sw.ElapsedMilliseconds/mmm);
            
            /*
            Parameter[] p = new Parameter[5];
            p[0] = new Parameter(99, 1, "начальный коэффициент", (x) => x / 100.0);
            p[1] = new Parameter(100, 1, "время обучения", (x) => x * 100.0);
            p[2] = new Parameter(20, 1, "Итерации обучения", (x) => x);
            p[3] = new Parameter(100, 1, "Число в 1 слое", (x) => x);
            p[4] = new Parameter(50, 2, "Число в 2 слое", (x) => x);
            FindAlgorithm finder = new FindAlgorithm(p, (x) => Console.WriteLine("Step: {0}", x), typeof(AGN), data, tupleSigment.Item1, tupleSigment.Item2);
            */
            /*
            Parameter[] p = new Parameter[5];
            p[0] = new Parameter(5, 1, "Число в 1 слое", (x) => x);
            p[1] = new Parameter(10, 1, "Число в 2 слое", (x) => x);
            p[2] = new Parameter(15, 1, "Итерации обучения", (x) => x);
            p[3] = new Parameter(13, 3, "S критерий", (x) => x);
            p[4] = new Parameter(81, 1, "x критерий", (x) => x/20.0);
           
            FindAlgorithm finder = new FindAlgorithm(p, (x, y) => Console.WriteLine("Step without best: {0}. Best count: {1}", x, y), typeof(AGRNN), data, tupleSigment.Item1, tupleSigment.Item2);
            
            object[] os = finder.Find().Item1;

            for (int i = 0; i < os.Length; i++ )
                Console.WriteLine(p[i].name+":"+os[i].ToString());
            */
            /*
            CVlog max = default(CVlog);
            
            bool flag = true;
            List<Tuple<double, double, int, int, int, int>> cools = new List<Tuple<double, double, int, int, int, int>>();

            //int one = 24; 
            int two = 9;
            double r = 0.21;
            double tm = 500;
            int s = 60;
            int m = 5;
            //for (double r = 0.15; r <= 0.25; r+=0.01 )
              //  for (double tm = 400; tm <= 600; tm += 200)
            for (int one = 1; one <= 24; one+=1)
              //  for (int two = 3; two <= 9; two+=1 )
            //for (int s = 2; s <= 20; s+=2 )
                //for (int m = 1; m <= 101; m += 5)
                {
                    Experiments experiment = new Experiments(() => new AGN(r, tm, m, one, two), 200);
                    //Experiments experiment = new Experiments(() => new AGRNN(one, two, m, s), 100);
                    //!Здесь вы можете присвоить другой классификатор - раскоментировать одну строчку и закомментировать другую! (если выбрали AGN - надо вызвать dispose в конце как внизу)
                    //new AGN(0.1, 1500);
                    //new RndA();
                    //new Regression(0.1, 3000);
                    CVlog t = experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}% (чекаем эпох: {1}, время {2}, r ={3}, ({4}, {5}) s - {6})", x * 100, m, tm, r, one, two, s); });
                    if (flag)
                    {
                        max = t;
                        cools.Add(new Tuple<double, double, int, int, int, int>(r, tm, m, one, two, s));
                        flag = false;
                    }
                    else
                    {
                        double tt = CVlog.Compare(t, max);
                        if (tt > 0.0)
                        {
                            Console.WriteLine("!!!Новый победитель: r = {0} t = {1} m = {2} ({3},{4}) s - {5}", r, tm, m, one, two, s);
                            max = t;
                            cools.Clear();
                            cools.Add(new Tuple<double, double, int, int, int, int>(r, tm, m, one, two, s));
                        }
                        else if (tt == 0.0)
                        {
                            cools.Add(new Tuple<double, double, int, int, int, int>(r, tm, m, one, two, s));
                            Console.WriteLine("Найден такой же хороший как предыдущий {0}", cools.Count);
                        }
                    }
                    //if (experiment.WriteLog(@"log" + m.ToString() + ".txt")) Console.WriteLine("Отчет сформирован");
                    //else Console.WriteLine("Не удалось");
                }

            List<Tuple<double, double, int, int, int, int>> cools2 = new List<Tuple<double, double, int, int, int, int>>();

            max = default(CVlog);

            foreach (Tuple<double, double, int, int, int, int> tuple in cools)
            {
                Experiments experiment = new Experiments(() => new AGN(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5), 1000);
                //Experiments experiment = new Experiments(() => new AGRNN(tuple.Item4, tuple.Item5, tuple.Item3, tuple.Item6), 1000);
                CVlog t = experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}% (чекаем 2 эпох: {1}, время {2}, r ={3}, ({4}, {5}), s - {6})", x * 100, tuple.Item3, tuple.Item2, tuple.Item1, tuple.Item4, tuple.Item5, tuple.Item6); });
                if (cools2.Count == 0)
                {
                    max = t;
                    cools2.Add(tuple);
                }
                else
                {
                    double tt = CVlog.Compare(t, max);
                    if (tt > 0.0)
                    {
                        Console.WriteLine("!!!Новый победитель: r = {0} t = {1} m = {2} ({3},{4}) s - {5}", tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);
                        max = t;
                        cools2.Clear();
                        cools2.Add(tuple);
                    }
                    else if (tt == 0.0)
                    {
                        cools2.Add(tuple);
                        Console.WriteLine("Найден такой же хороший как предыдущий {0}", cools2.Count);
                    }
                }
            }

            Console.WriteLine("Список хороших:");
            var writer = new StreamWriter("best.txt", false);
            foreach (Tuple<double, double, int, int, int, int> tuple in cools2)
            {
                Console.WriteLine("r={0}; t = {1}; m = {2} ({3}, {4}, s - {5})", tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);
                writer.WriteLine("r={0}; t = {1}; m = {2} ({3}, {4}), s - {5}", tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);
            }
                

            writer.Close();
            writer.Dispose();
            */
            //GC.Collect();
            Console.ReadKey();
        }
    }
}