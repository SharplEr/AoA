using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using Araneam;
using GenomeNeuralNetwork;

namespace ConsoleGenomeRuner
{
    class Program
    {
        static void Main(string[] args)
        {

            GenomeNetwork gnw = new GenomeNetwork(0.5, 1000);

            Console.WriteLine("Старт обучения");
            string[] s = new string[] { @"D:\data2.csv", @"D:\data1.csv"};
            //s[0] = @"D:\data1.csv";
            //s[1] = @"D:\data2.csv";
            if (gnw.Reload(s)) gnw.EarlyStoppingLearn();
            else Console.WriteLine("Что-то не так");
        }
    }
}
