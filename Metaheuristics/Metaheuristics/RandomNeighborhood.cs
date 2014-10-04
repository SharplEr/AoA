using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    class RandomNeighborhood:INeighborhood
    {
        Random random;
        Parameter[] parameters;
        double[] delta;

        public RandomNeighborhood(Random r, Parameter[] ps)
        {
            random = r;
            parameters = ps;
            delta = new double[parameters.Length];
            for (int i = 0; i < delta.Length; i++)
                delta[i] = 1;    //1 процент
        }

        public int[] Get(int[] x)
        {
            int[] ans = new int[x.Length];

            for(int i = 0; i< ans.Length; i++)
            {
                int d = (int)Math.Round(delta[i] * (parameters[i].length + 1));
                int max = x[i]+1 + d;
                int min = x[i]-1-d;

                if (max > parameters[i].max) max = parameters[i].max;
                if (min < parameters[i].min) min = parameters[i].min;

                ans[i] = random.Next(min, max + 1);
            }
            ans[0] = 1;
            return ans;
        }
    }
}
