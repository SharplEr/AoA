using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public class AnnealingJump: IJump
    {
        Random random;
        Func<double> T;

        public AnnealingJump(Random r, Func<double> t)
        {
            random = r;
            T = t;
        }

        public bool Jump(double x, double y)
        {
            if (y >= x) return true;

            double test = random.NextDouble();

            if (test < Math.Exp((y - x) / T())) return true;
            else return false;
        }
    }
}
