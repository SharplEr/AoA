using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    class AnnealingNeighborhood: INeighborhood
    {
        Random random;
        Parameter[] parameters;
        Func<double>[] temp;

        public AnnealingNeighborhood(Random r, Parameter[] ps, Func<double>[] t)
        {
            random = r;
            parameters = ps;
            temp = t;
        }

        public int[] Get(int[] x)
        {
            int[] ans = new int[x.Length];

            for(int i = 0; i< ans.Length; i++)
            {
                ans[i] = x[i];
                if (parameters[i].length != 0)
                {
                    double z = random.NextDouble();
                    double t = temp[i]();
                    z = Math.Sign(z - 0.5) * t * (Math.Pow(1.0 + 1.0 / t, Math.Abs(2 * z - 1)) - 1);

                    //Если требуется найти t, по известному z, то t=-z*z/(2*z-1), при z0 = 0.75.
                    ans[i] = (int)Math.Round(z * (parameters[i].length));

                    if (ans[i] > parameters[i].max) ans[i] = parameters[i].max;
                    else if (ans[i] < parameters[i].min) ans[i] = parameters[i].min;
                }
            }

            return ans;
        }
    }
}