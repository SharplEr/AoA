using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public interface IQuality<T> where T : IQuality<T>
    {
        double CompareTo(T o);
    }
}