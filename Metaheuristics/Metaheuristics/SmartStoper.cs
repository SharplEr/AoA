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
            //return f() >= min;
            //Хорошо бы как-то сделать так, что бы нельзя было топтаться на одном месте
            //Ну список запрета наверное затащит, но его же хер сделаешь нормально... хотя я и так наверное не особо топтаюсь
            int m = (int)Math.Round(Math.Sqrt(maxStep));
            if (m < 2) m = 2;
            return (Step() < maxStep) || (StepWithoutBest() < m) || (f()>min);
            //Так как поле дискретное, то и нет особой мазы ограничивать общее число итераций - пока находит, пусть ищет.
        }
    }
}
