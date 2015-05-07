using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using AoA;
using IOData;
using Metaheuristics;

namespace AoARun
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Запущенно со следующими параметрами: "+ String.Join(", ", args));

            Console.WriteLine("Файл описывающий данные: {0}. Существует: {1}.", args[0], File.Exists(args[0]));
            if (!File.Exists(args[0])) return;

            Console.WriteLine("Файл описывающий реализацию алгоритма: {0}. Существует: {1}.", args[1], File.Exists(args[1]));
            if (!File.Exists(args[1])) return;

            Console.WriteLine("Выходной файл: {0}.", args[2]);

            FullData data = new FullData(args[0]);
            
            double k = 0.2;

            if (args.Length > 3)
            {
                try
                {
                    k = Convert.ToDouble(args[3], NumberFormatInfo.InvariantInfo);
                }
                catch
                {
                    k = Convert.ToDouble(args[3]);
                }
            }
            Console.WriteLine("Доля на контроле = {0}. Число на обучении = {1}", k, data.Length - (int)Math.Round(data.Length * k));

            if ((int)Math.Round(data.Length * k) < 2) Console.WriteLine("Активирован режим скользящего контроля с одним отделяемым объектом");
            
            int mmm = 180;

            if (args.Length > 4)
                    mmm = Convert.ToInt32(args[4]);

            Console.WriteLine("Итараций скользящего контроля при поиске: {0}", mmm);

            var tupleSigment = DataManager.GetShuffleFrom(data, mmm, k, new Random(271828314));

            int m5m = 5 * mmm;

            if (mmm < 1)
            {
                m5m = 0;
                Console.WriteLine("Скользящий контроль отключен");
            }
            else
                if (args.Length > 5)
                    mmm = Convert.ToInt32(args[5]);

            Console.WriteLine("Итараций скользящего контроля в конце: {0}", m5m);

            //0,21-500-5-24-9
            /*
            Experiments experiment = new Experiments(() => new ThreeLayerNetwork(0.02, 50, 3, 24, 9), 150, mmm);

            //1-2-10-6
            //1-6-1-13-2,05
            //Experiments experiment = new Experiments(() => new AGRNN(4,8, 7, 3, 2.05), 150, mmm);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CVlog v = experiment.Run(data, (x) => { Console.WriteLine("Завершено {0}%", x * 100); }, tupleSigment.Item1, tupleSigment.Item2);
            sw.Stop();

            if (v.Save(new StreamWriter(@"log.txt", false))) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");
            
            Console.WriteLine("Время: {0} (мс/обучение)", (double)sw.ElapsedMilliseconds/mmm);
            
            */
            /*
            const double ir = Math.PI / 7.0 * 22.0;
            const double ir2 = Math.E * 71.0 / 193.0;
            Parameter[] p = new Parameter[3];
            p[0] = new Parameter(500, 1, "начальный коэффициент", (x) => ir * x / 500.0);
            //p[0] = new Parameter(60, 1, "начальный коэффициент", (x) => 0.01);
            p[1] = new Parameter(32, 1, "время обучения", (x) => ir2 * x * 2.0);
            //p[1] = new Parameter(1, 1, "время обучения", (x) => 0.0);
            p[2] = new Parameter(10, 1, "Эпох обучения", (x) => x);
            //p[3] = new Parameter(40, 3, "Число в 1 слое", (x) => x);
            //p[4] = new Parameter(40, 3, "Число в 2 слое", (x) => x);
            //p[5] = new Parameter(40, 3, "Число в 3 слое", (x) => x);
            //p[6] = new Parameter(40, 3, "Число в 4 слое", (x) => x);

            Type type = typeof(Regression);
            //typeof(AGRNN)//ThreeLayerNetwork//TwoLayerNetwork//MPL///Regression//Neighbour*/

            Console.WriteLine("Загружаем алгоритм из dll...");
            var dll = AlgorithmFactory.LoadAlgorithmInfo(args[1]);

            Parameter[] p = dll.Item2;
            Type type = dll.Item1;
            Console.WriteLine("Алгоритм: {0}, параметры: {1}.", type, dll.Item2);
            int step = 0;
            object[] os = null;

            if (p != null)
            {

                var finder = new FindAlgorithm(p,
                    (x, y) => Console.WriteLine("Step without best: {0}. Best count: {1}. All step: {2}", x, y, step++),
                    type, data, tupleSigment.Item1, tupleSigment.Item2);

                Console.WriteLine("Поехали искать");

                var ans = finder.Find();

                os = ans.Item1;
                ans.Item2.Dispose(); //Второй элемент не нужен

                for (int i = 0; i < os.Length; i++)
                    Console.WriteLine(p[i].name + ":" + os[i].ToString());
            }

            using (var writer = new StreamWriter(args[2], false))
            {
                writer.WriteLine("      Начало отчета алгоритма {0}", type);

                if (os != null)
                {
                    writer.WriteLine("Найденные параметры:");
                    for (int i = 0; i < os.Length; i++)
                        writer.WriteLine(p[i].name + ":" + os[i].ToString());
                    writer.WriteLine("Шагов поиска выполнено: {0}", step);
                }

                writer.WriteLine();

                using (var exp = new Experiments(() => AlgorithmFactory.GetFactory(type)(os), 150))
                {

                    var ts = DataManager.GetShuffleFrom(data, 5*mmm, k, new Random(271828314));

                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    var log = exp.Run(data, (x) => Console.WriteLine("завершающие проценты {0}", x), ts.Item1, ts.Item2);
                    sw.Stop();
                    Console.WriteLine("Последний проход выполнен за {0:F} с", (double) sw.ElapsedMilliseconds/1000);
                    log.Save(writer);
                    log.Dispose();
                }
            }
            GC.Collect();
            Console.ReadKey();
        }
    }
}