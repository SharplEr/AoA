using System;
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

            Experiments experiment = new Experiments(data.Length, () => new AGN(0.1, 5500, 49));
            experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}%", x * 100); });
            if (experiment.WriteLog(@"log.txt")) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");

            
            /*
            CVlog max = default(CVlog);
            
            bool flag = true;
            List<Tuple<double, double, int>> cools = new List<Tuple<double, double, int>>();
            //for (double r = 0.01; r <= 0.31; r+=0.1 )
            double r = 0.1;
            double tm = 5500;
                //for (double tm = 5000; tm <= 6000; tm += 500)
                    for (int m = 48; m <= 70; m+=1)
                    {
                        Experiments experiment = new Experiments(input.Length, () => new AGN(r, tm, m));

                        //!Здесь вы можете присвоить другой классификатор - раскоментировать одну строчку и закомментировать другую! (если выбрали AGN - надо вызвать dispose в конце как внизу)
                        //new AGN(0.1, 1500);
                        //new RndA();
                        //new Regression(0.1, 3000);
                        CVlog t = experiment.Run(input, output, (x) => { Console.WriteLine("Завершено {0}% (чекаем эпох: {1}, время {2}, r ={3})", x * 100, m, tm, r); });
                        if (flag)
                        {
                            max = t;
                            cools.Add(new Tuple<double, double, int>(r, tm, m));
                            flag = false;
                        }
                        else
                        {
                            double tt = CVlog.Compare(t, max);
                            if (tt > 0.0)
                            {
                                Console.WriteLine("новый победитель: r = {0} t = {1} m = {2}", r, tm, m);
                                max = t;
                                cools.Clear();
                                cools.Add(new Tuple<double, double, int>(r, tm, m));
                            }
                            else if (tt == 0.0)
                            {
                                Console.WriteLine("Найден такой же хороший как предыдущий");
                                cools.Add(new Tuple<double, double, int>(r, tm, m));
                            }
                        }

                        //if (experiment.WriteLog(@"log" + m.ToString() + ".txt")) Console.WriteLine("Отчет сформирован");
                        //else Console.WriteLine("Не удалось");
                    }
            
            Console.WriteLine("Список хороших:");
            foreach (Tuple<double, double, int> tuple in cools)
                Console.WriteLine("r={0}; t = {1}; m = {2}", tuple.Item1, tuple.Item2, tuple.Item3);
            */
            Console.ReadKey();
        }
    }
}
