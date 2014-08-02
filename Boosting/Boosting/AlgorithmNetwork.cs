using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoA;
using VectorSpace;

namespace Boosting
{
    /*
    public class AlgorithmNetwork
    {
        Algorithm[] layer;
        Algorithm hade;

        public Vector Calc(Vector input, int dem)
        {
            Vector newInput = new Vector(input.Length + layer.Length*dem);

            for (int i = 0; i < input.Length; i++)
                newInput[i] = input[i];

            for (int i = 0; i < layer.Length; i++)
            {
                Vector v = layer[i].Calc(input);
                int start = input.Length + dem * i;
                for (int j = 0; j < dem; j++)
                    newInput[start + j] = v[j];
            }
            return hade.Calc(newInput);
        }
        /*
        //Для двух алгоритмов в первом слое (пока)
        public AlgorithmNetwork(Vector[] inputData, Vector[] outputData, Func<double[],Algorithm>[] getAlgorithmes)
        {
            //Проверка входных данных
            if (inputData == null) throw new ArgumentNullException();
            if (inputData[0] == null) throw new ArgumentNullException();
            if (getAlgorithmes == null) throw new ArgumentNullException();
            if (getAlgorithmes[] == null) throw new ArgumentNullException();

            layer = new Algorithm[2];
            //Поиск первого алгоритма (лучшего из возможных):
            Experiments CV = new Experiments(inputData.Length, ()=>getAlgorithmes[0](null));

            CVlog log = CV.Run(inputData, outputData, (x)=>Console.WriteLine("1:0: {0}", x*100));
            int max = 0;
            for(int i = 1; i<getAlgorithmes.Length; i++)
            {
                CV = new Experiments(inputData.Length, ()=>getAlgorithmes[0](null));
                CVlog tlog = CV.Run(inputData, outputData, (x)=>Console.WriteLine("1:{1}: {0}", x*100, i));
                if (CVlog.Compare(log, tlog)<0)
                {
                    max = i;
                    log = tlog;
                }
            }

            //Определение весовых коэффициентов
            double maxError = 0.0;
            for(int i = 0; i< log.avgErrorAtControls.Length; i++)
                if (log.avgErrorAtControls[i]>maxError) maxError = log.avgErrorAtControls[i];

            double[] rating = new double[inputData.Length];
            rating = (double[])log.avgErrorAtControls.Clone();
            for(int i = 0; i< rating.Length; i++)
                rating[i]/=maxError;

            double[] inversRating = (double[])rating.Clone();
            for(int i = 0; i< inversRating.Length; i++)
                inversRating[i] = 1.0/inversRating[i];
            maxError = 0.0;
            for(int i = 0; i< inversRating.Length; i++)
                if (inversRating[i]>maxError) maxError = inversRating[i];
            for(int i = 0; i< inversRating.Length; i++)
                inversRating[i]/=maxError;

            layer[0] = getAlgorithmes[max](inversRating);
            layer[0].Learn(inputData, outputData);
            //Поиск второго алгоритма


            //Обучение главного алгоритма

            //Experiments analis = new Experiments(inputData.Length, 
        }*/
    //}*/
}
