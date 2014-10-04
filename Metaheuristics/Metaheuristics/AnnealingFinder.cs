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

        public AnnealingFinder(Parameter[] p, Action<int> w): base(p, w)
        {
            const double t0 = 2;
            const double c = 1;
            temp = () => t0 * Math.Exp(-c * Math.Pow(step, 1.0 / parameters.Length));
            
            double m = 0.0;

            for (int i = 0; i < p.Length; i++)
                m += (p[i].length + 1)*(p[i].length + 1);

            m = Math.Sqrt(m) / 2;
            if (m < 1) m = 1;
            starter = new RandomStart(random, p);
            stoper = new SmartStoper(()=> step, ()=> stepWithoutBest, (int)Math.Round(m), temp, 0.01);
            
            jumper = new AnnealingJump<T>(random, temp);
            neighbor = new AnnealingNeighborhood(random, p, temp);
        }
    }
}
