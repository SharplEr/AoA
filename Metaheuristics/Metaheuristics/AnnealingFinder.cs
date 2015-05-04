using System;
using System.Collections.Generic;
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

        protected AnnealingFinder(Parameter[] p, Action<int, int> w, double maxDelta, int maxStep)
            : base(p, w)
        {
            double d = Math.Pow(maxStep, 1.0 / parameters.Length);
            temps = new Func<double>[p.Length];
            int m = 1;
            for (int i = 0; i < p.Length; i++)
            {
                //m += (p[i].length + 1) * (p[i].length + 1);
                if (p[i].length > 1) m *= 2;
                else if (p[i].length > 0) m += 1;
                int L = parameters[i].length;// +1;
                double cl = Math.Log((L * L) * (1.0 - 2.0 / L)) / d;
                temps[i] = () => Math.Exp(-cl * Math.Pow(step, 1.0 / parameters.Length));
            }

            m = (int)Math.Round(Math.Sqrt(m) / 2);
            if (m < p.Length) m = p.Length;
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
