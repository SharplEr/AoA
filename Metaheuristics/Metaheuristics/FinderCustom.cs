using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public abstract class FinderCustom<T> : Finder<T> where T : IQuality<T>
    {
        protected IGoStart starter;
        protected IDontStop stoper;
        protected IJump<T> jumper;
        protected INeighborhood neighbor;

        public FinderCustom(Parameter[] p, Action<int> w): base(p, w)
        {}

        protected override void GoStart()
        {
            starter.GetStart(position);

            //Дальнейший код дублируется -- когда-то придется его выносить, но как лучше?
            bestPosition = (int[])position.Clone();
            bestResult = Quality(bestPosition);
            positionResult = bestResult;
            step = 0;
            stepWithoutBest = 0;
        }

        protected override bool DontStop()
        {
            return stoper.DontStop();
        }

        protected override bool Jump(T x, T y)
        {
            return jumper.Jump(x, y);
        }

        protected override int[] Neighborhood(int[] x)
        {
            return neighbor.Get(x);
        }
    }
}