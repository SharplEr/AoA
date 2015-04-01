using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public class Stoper : IDontStop
    {
        readonly int maxStep;
        readonly Func<int> Step;

        public Stoper(Func<int> step, int maxS)
        {
            Step = step;
            maxStep = maxS;
        }

        public bool DontStop()
        {
            return Step() < maxStep;
        }
    }
}