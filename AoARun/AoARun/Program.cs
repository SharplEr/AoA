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