using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public class AnnealingJump<T> : IJump<T> where T : IQuality<T>
    {
        Random random;
        Func<double> Temp;

        public AnnealingJump(Random r, Func<double> t)
        {
            random = r;
            Temp = t;
        }

        public bool Jump(T x, T y)
        {
            double delta = y.CompareTo(x);

            if (delta >= 0) return true;

            double test = random.NextDouble();

            if (test < Math.Exp((delta) / Temp())) return true;
            else return false;
        }
    }
}
