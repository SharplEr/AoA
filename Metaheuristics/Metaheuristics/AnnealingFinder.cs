using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    /// <summary>
    /// Поиск алгоритмом имитации отжига
    /// </summary>
    public abstract class AnnealingFinder:FinderCustom
    {
        Random random = new Random();

        AnnealingFinder(Parameter[] p, int m): base(p)
        {
            starter = new RandomStart(random, p);
            stoper = new SmartStoper(
                () => step,
                () => stepWithoutBest,
                m
                );
            jumper = new AnnealingJump(random,
                () => 1.0 / step
                );
            neighbor = new RandomNeighborhood(random, p);
        }
    }
}
