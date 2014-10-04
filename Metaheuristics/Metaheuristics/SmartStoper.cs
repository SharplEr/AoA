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
        Func<double> f;
        double min;
        public SmartStoper(Func<int> step, Func<int> stepWithoutBest, int maxS, Func<double> f, double min)
        {
            Step = step;
            StepWithoutBest = stepWithoutBest;
            maxStep = maxS;
            this.f = f;
            this.min = min;
        }

        public bool DontStop()
        {
            //Надо дать алгоритму шанс по дольше поискать
            return (Step() < 2 * maxStep) || (StepWithoutBest() < maxStep) && (f()>min);
            //Так как поле дискретное, то и нет особой мазы ограничивать общее число итераций - пока находит, пусть ищет.
        }
    }
}
