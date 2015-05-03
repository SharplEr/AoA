using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public class TrueJumper<T>: IJump<T> where T : IQuality<T>
    {
        public TrueJumper()
        { }

        public bool Jump(T x, T y)
        {
            return true;
        }
    }
}
