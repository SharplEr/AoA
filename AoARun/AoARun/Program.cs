using System;
using IODate;
using VectorSpace;
using AoA;
using GenomeNeuralNetwork;

namespace AoARun
{
    class Program
    {
        static void Main(string[] args)
        {
            DateAnalysis analysis = new DateAnalysis(
                new string[] { @"..\..\..\Data\data_1.csv", @"..\..\..\Data\data_2.csv" }
                , GenomeNetwork.TestTags, GenomeNetwork.ResultTags, GenomeNetwork.FenTags, (s) =>
            {
                if (s[0] == "отрицат") return -1.0;
                else return 1.0;
            }, GenomeNetwork.ToDouble);

            Vector[] input = analysis.TestDate;
            Vector[] output = analysis.ResultDate;

            //Experiments experiment = new Experiments(input.Length, () => new AGN(0.1, 1500));
            Experiments experiment = new Experiments(input.Length, () => new AGN(0.1, 1500));

            //!Здесь вы можете присвоить другой классификатор - раскоментировать одну строчку и закомментировать другую! (если выбрали AGN - надо вызвать dispose в конце как внизу)
            //new AGN(0.1, 1500);
            //new RndA();
            //new Regression(0.1, 3000);
            //experiment.Run(input, output, (x) => { Console.WriteLine("Завершено {0}%", x * 100); });
            experiment.Run(input, output, (x) => { Console.WriteLine("Завершено {0}%", x * 100); });
            //, (x) => { Console.WriteLine("Завершено {0}%", x * 100); }
            if (experiment.WriteLog(@"log.txt")) Console.WriteLine("Отчет сформирован");
            else Console.WriteLine("Не удалось");

            Console.ReadKey();
        }
    }
}
