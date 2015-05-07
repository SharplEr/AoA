using System;

namespace Metaheuristics
{
    
    public abstract class RandomFinder<T> : FinderCustom<T> where T : IQuality<T>
    {
        readonly int maxStep;

        readonly Random random = new Random();

        protected RandomFinder(Parameter[] p, Action<int, int> w, int mxstep)
            : base(p, w)
        {
            maxStep = mxstep;

            starter = new RandomStart(random, p);

            stoper = new Stoper(() => step, maxStep);
            
            jumper = new TrueJumper<T>();

            neighbor = new RandomNeighborhood(random, p);
        }

    }
}
