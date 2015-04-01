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

        public RandomNeighborhood(Random r, Parameter[] ps)
        {
            random = r;
            parameters = ps;
        }

        public int[] Get(int[] x)
        {
            int[] ans = new int[x.Length];

            for(int i = 0; i< ans.Length; i++)
                ans[i] = random.Next(parameters[i].min, parameters[i].max+1);

            return ans;
        }
    }
}
