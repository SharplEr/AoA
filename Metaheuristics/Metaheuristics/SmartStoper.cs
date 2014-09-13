using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    class SmartStoper : IDontStop
    {
        Func<int> Step;
        Func<int> StepWithoutBest;
        int maxStep;

        public SmartStoper(Func<int> step, Func<int> stepWithoutBest, int maxS)
        {
            Step = step;
            StepWithoutBest = stepWithoutBest;
            maxStep = maxS;
        }

        public bool DontStop()
        {
            //Надо дать алгоритму шанс по дольше поискать
            return (Step() < 2 * maxStep) || (StepWithoutBest() < maxStep);
            //Так как поле дискретное, то и нет особой мазы ограничивать общее число итераций - пока находит, пусть ищет.
        }
    }
}
