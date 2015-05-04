using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public abstract class Sieve<T> where T : IQuality<T>
    {
        Parameter[] parameters;

        Action<int, double> Whatup;
        int[] position;

        T bestResults;

        /// <param name="p">Параметры</param>
        /// <param name="w">Возвращает глубину, и процент обработки</param>
        protected Sieve(Parameter[] p, Action<int, double> w)
        {
            parameters = p;
            position = new int[parameters.Length];
            Whatup = w;
        }
        /*
        public virtual Tuple<object[], T> Find(int start, int iteration, double k)
        {
            return new Tuple<object[], T>(Convert(FindRaw(start, iteration, k)), bestResults);
        }*/

        /*
        protected virtual int[] FindRaw(int start, int iteration, double k)
        {
            Random random = new Random();

            for (int i = 0; i < position.Length; i++)
                position[i] = random.Next(parameters[i].min, parameters[i].max + 1);

            for(int i = 0; i < start)


            return bestPosition;
        }*/

        protected abstract T Quality(object[] x);

        protected object[] Convert(int[] x)
        {
            object[] y = new object[x.Length];

            for (int i = 0; i < x.Length; i++)
                y[i] = parameters[i].convert(x[i]);

            return y;
        }
    }
}
