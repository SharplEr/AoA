using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    /// <summary>
    /// Базовый класс для одного метаэвристического агента
    /// </summary>
    public abstract class Finder
    {
        protected Parameter[] parameters;
        protected int[] position;
        protected int[] bestPosition;
        protected double bestResult;

        protected int step;
        protected int stepWithoutBest;

        public Finder(Parameter[] p)
        {
            parameters = p;
            
            position = new int[parameters.Length];
        }

        protected virtual void GoStart()
        {
            //По умолчанию становимся в центре
            for (int i = 0; i < position.Length; i++)
                position[i] = parameters[i].min + parameters[i].length / 2;

            bestPosition = (int[])position.Clone();
            bestResult = Quality(bestPosition);
            step = 0;
            stepWithoutBest = 0;
        }

        protected virtual void GoNext()
        {
            int[] newPosition = Neighborhood(position);

            double x = Quality(position);
            double y = Quality(newPosition);

            if (Jump(x, y))
                position = newPosition;

            //Мало ли не во всех алгоритмах будет переход к лучшему решению. Может иногда будет создатьваться другой Finder для того что бы там посмотреть
            if (y > bestResult)
            {
                bestPosition = (int[])newPosition.Clone();
                bestResult = y;

                stepWithoutBest = 0;
            }
            else stepWithoutBest++;

            step++;
        }

        protected virtual bool DontStop()
        {
            return stepWithoutBest < 10;
        }

        protected virtual int[] FindRaw()
        {
            GoStart();

            while (DontStop())
            {
                GoNext();
                step++;
            }

            return bestPosition;
        }

        public virtual object[] Find()
        {
            return Convert(FindRaw());
        }

        /// <summary>
        /// Окрестность точки x
        /// </summary>
        protected abstract int[] Neighborhood(int[] x);

        //Реализация должна знать какой object что значит и как его юзать
        protected abstract double Quality(object[] x);

        protected object[] Convert(int[] x)
        {
            object[] y = new object[x.Length];

            for (int i = 0; i < x.Length; i++)
                y[i] = parameters[i].convert(x[i]);

            return y;
        }

        /// <summary>
        /// Полезность точки x
        /// </summary>
        protected double Quality(int[] x)
        {
            return Quality(Convert(x));
        }

        /// <summary>
        /// Совершать ли переход?
        /// </summary>
        protected virtual bool Jump(double x, double y)
        {
            //По умолчанию выбираем лучший
            if (y >= x) return true;
            else return false;
        }
    }
}