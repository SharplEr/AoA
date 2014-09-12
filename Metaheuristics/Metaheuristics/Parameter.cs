using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public class Parameter
    {
        public readonly string name;
        public readonly int max;
        public readonly int min;
        public readonly Func<int, object> convert;
        /// <summary>
        /// max-min
        /// </summary>
        public readonly int length; //Постоянно нужно, так что бы не добавить?

        public Parameter(int mx, int mn, string n, Func<int, object> f)
        {
            max = mx;
            min = mn;
            name = n;
            convert = f;
            length = max - min;
        }
    }
}
