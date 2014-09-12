using System;
using System.IO;
using IOData;
using VectorSpace;
using AoA;
using GenomeNeuralNetwork;
using System.Collections.Generic;

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
            
            //21,30,11-0,641+0,005
            //35-24-9-0,664-лучшее, при 0.1, 5500
            //
            /*
            //Experiments experiment = new Experiments(data.Length, () => new AGN(0.21, 500, 29, 24, 9));
            Experiments experiment = new Experiments(data.Length, () => new AGRNN(20,8,10));
            experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}%", x * 100); });
            if (experiment.WriteLog(@"log.txt")) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");
            */
            
            
            CVlog max = default(CVlog);
            
            bool flag = true;
            List<Tuple<double, double, int, int, int>> cools = new List<Tuple<double, double, int, int, int>>();

            //int one = 24; int two = 9;
            double r = 0.21;
            double tm = 500;
            //for (double r = 0.15; r <= 0.25; r+=0.01 )
              //  for (double tm = 400; tm <= 600; tm += 200)
            for (int one = 10; one <= 30; one+=1)
                for (int two = 3; two <= 15; two+=1 )            
                    for (int m = 10; m <= 29; m += 1)
                    {
                        Experiments experiment = new Experiments(data.Length, () => new AGN(r, tm, m, one, two));
                        //Experiments experiment = new Experiments(data.Length, () => new AGRNN(one, two, m));
                        //!Здесь вы можете присвоить другой классификатор - раскоментировать одну строчку и закомментировать другую! (если выбрали AGN - надо вызвать dispose в конце как внизу)
                        //new AGN(0.1, 1500);
                        //new RndA();
                        //new Regression(0.1, 3000);
                        CVlog t = experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}% (чекаем эпох: {1}, время {2}, r ={3}, ({4}, {5}))", x * 100, m, tm, r, one, two); });
                        if (flag)
                        {
                            max = t;
                            cools.Add(new Tuple<double, double, int, int, int>(r, tm, m, one, two));
                            flag = false;
                        }
                        else
                        {
                            double tt = CVlog.Compare(t, max);
                            if (tt > 0.0)
                            {
                                Console.WriteLine("новый победитель: r = {0} t = {1} m = {2} ({3},{4})", r, tm, m, one, two);
                                max = t;
                                cools.Clear();
                                cools.Add(new Tuple<double, double, int, int, int>(r, tm, m, one, two));
                            }
                            else if (tt == 0.0)
                            {
                                Console.WriteLine("Найден такой же хороший как предыдущий");
                                cools.Add(new Tuple<double, double, int, int, int>(r, tm, m, one, two));
                            }
                        }

                        //if (experiment.WriteLog(@"log" + m.ToString() + ".txt")) Console.WriteLine("Отчет сформирован");
                        //else Console.WriteLine("Не удалось");
                    }
            
            Console.WriteLine("Список хороших:");
            foreach (Tuple<double, double, int, int, int> tuple in cools)
                Console.WriteLine("r={0}; t = {1}; m = {2} ({3}, {4})", tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

            var writer = new StreamWriter("best1.txt", false);

            foreach (Tuple<double, double, int, int, int> tuple in cools)
                writer.WriteLine("r={0}; t = {1}; m = {2} ({3}, {4})", tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

            writer.Close();
            writer.Dispose();
            //GC.Collect();
            Console.ReadKey();
        }
    }
}
