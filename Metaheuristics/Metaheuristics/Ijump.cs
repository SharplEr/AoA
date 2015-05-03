using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public interface IJump<T> where T : IQuality<T>
    {
        bool Jump(T x, T y);
    }
}
