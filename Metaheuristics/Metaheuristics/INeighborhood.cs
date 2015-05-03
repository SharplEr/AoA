using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public interface INeighborhood
    {
        int[] Get(int[] x);
    }
}
