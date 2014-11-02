using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    /// <summary>
    /// Поиск алгоритмом имитации отжига
    /// </summary>
    public abstract class AnnealingFinder<T> : FinderCustom<T> where T : IQuality<T>
    {
        protected Random random = new Random();

        protected Func<double> temp;
        protected Func<double>[] temps;

        public AnnealingFinder(Parameter[] p, Action<int, int> w, double maxDelta, int maxStep): base(p, w)
        {
            double d = Math.Pow(maxStep, 1.0 / parameters.Length);
            temps = new Func<double>[p.Length];
            int m = 1;
            for (int i = 0; i < p.Length; i++)
            {
                //m += (p[i].length + 1) * (p[i].length + 1);
                if (p[i].length>0) m *= 2;
                double cl = Math.Log((parameters[i].length * parameters[i].length) * (1.0 - 2.0 / parameters[i].length))/d;
                temps[i] = () => Math.Exp(-cl * Math.Pow(step, 1.0 / parameters.Length));
            }

            //m = Math.Sqrt(m) / 2;
            //if (m < 1) m = 1;
            starter = new RandomStart(random, p);
            
            double epsilon = 0.001; //Вероятность перехода в плохое решение на последних шагах алгоритма
            double minTemp = -maxDelta / Math.Log(epsilon);
            double maxTemp = -maxDelta/Math.Log(1-epsilon);
            double c = Math.Log(maxTemp / minTemp) / d;
            temp = () => maxTemp * Math.Exp(-c * Math.Pow(step, 1.0 / parameters.Length));

            stoper = new SmartStoper(() => step, () => stepWithoutBest, m, temp, minTemp);

            jumper = new AnnealingJump<T>(random, temp);

            neighbor = new AnnealingNeighborhood(random, p, temps);
        }
    }
}
