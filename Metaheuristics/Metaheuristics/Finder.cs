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
    public abstract class Finder<T> where T : IQuality<T>
    {
        protected Parameter[] parameters;
        protected int[] position;
        protected T positionResult;
        protected int[] bestPosition;
        protected T bestResult;

        protected int step;
        protected int stepWithoutBest;
        protected int stepWithoutJump;

        protected Action<int> Whatup;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="p">Список параметров</param>
        public Finder(Parameter[] p, Action<int> w)
        {
            parameters = p;
            
            position = new int[parameters.Length];
            Whatup = w;
        }

        protected virtual void GoStart()
        {
            //По умолчанию становимся в центре
            for (int i = 0; i < position.Length; i++)
                position[i] = parameters[i].min + parameters[i].length / 2;

            bestPosition = (int[])position.Clone();
            bestResult = Quality(bestPosition);
            positionResult = bestResult;
            step = 0;
            stepWithoutBest = 0;
            stepWithoutJump = 0;
        }


        int count = 0;

        protected virtual void ChangeStep(bool jump, bool best)
        {
            if (best) stepWithoutBest = 0;
            else stepWithoutBest++;

            if (jump)
            {
                step++;
                stepWithoutJump = 0;
            }
            else
            {
                stepWithoutJump++;

                if (stepWithoutJump > 10)
                    step++;
            }
        }

        protected virtual void GoNext()
        {
            int[] newPosition = Neighborhood(position);

            T y = Quality(newPosition);
            bool jump = Jump(positionResult, y);
            if (jump)
            {
                position = newPosition;
                positionResult = y;
                //step++;
                //stepWithoutJump = 0;
            }
            //else stepWithoutJump++;

            //if (stepWithoutJump > 10) step++;

            //Мало ли не во всех алгоритмах будет переход к лучшему решению. Может иногда будет создатьваться другой Finder для того что бы там посмотреть
            double delta = y.CompareTo(bestResult);
            bool best = delta > 0;
            if (best)
            {
                Console.WriteLine("Нашли по лучше! {0}", ++count);
                bestPosition = (int[])newPosition.Clone();
                bestResult = y;

                //stepWithoutBest = 0;
            }
            else
            {
                //stepWithoutBest++;
                Console.WriteLine("Не хватило: {0}", delta);
            }

            Console.WriteLine("Лучших найдено: {0}", count);

            ChangeStep(jump, best);

            Whatup(stepWithoutBest);
        }

        /// <summary>
        /// Не пора ли остановиться?
        /// </summary>
        protected virtual bool DontStop()
        {
            return stepWithoutBest < 10;
        }

        protected virtual int[] FindRaw()
        {
            //Ключевой код поиска
            GoStart();

            while (DontStop())
            {
                GoNext();
            }

            return bestPosition;
        }

        public virtual Tuple<object[], T> Find()
        {
            return new Tuple<object[],T>(Convert(FindRaw()), bestResult);
        }

        /// <summary>
        /// Окрестность точки x
        /// </summary>
        protected abstract int[] Neighborhood(int[] x);
        
        //Реализация должна знать какой object что значит и как его юзать
        protected abstract T Quality(object[] x);

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
        protected T Quality(int[] x)
        {
            return Quality(Convert(x));
        }

        /// <summary>
        /// Совершать ли переход?
        /// </summary>
        protected virtual bool Jump(T x, T y)
        {
            //По умолчанию выбираем лучший
            if (y.CompareTo(x) < 0) return false;
            else return true;
        }
    }
}