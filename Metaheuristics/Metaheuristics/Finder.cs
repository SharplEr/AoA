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
        Parameter[] parameters;
        int[] position;
        int[] bestPosition;
        double bestResult;

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
            bestResult = utility(bestPosition);
        }

        protected virtual void GoNext()
        {
            int[] newPosition = Neighborhood(position);

            double x = utility(position);
            double y = utility(newPosition);

            if (jump(x, y))
                position = newPosition;

            if (y > bestResult)
            {
                bestPosition = (int[])newPosition.Clone();
                bestResult = y;
            }
        }

        public virtual int[] Find()
        {
            GoStart();

            return position;
        }

        /// <summary>
        /// Окрестность точки x
        /// </summary>
        protected abstract int[] Neighborhood(int[] x);

        //Реализация должна знать какой object что значит и как его юзать
        protected abstract double utility(object[] x);

        /// <summary>
        /// Полезность точки x
        /// </summary>
        protected double utility(int[] x)
        {
            object[] y = new object[x.Length];

            for (int i = 0; i < x.Length; i++)
                y[i] = parameters[i].convert(x[i]);

            return utility(y);
        }

        /// <summary>
        /// Совершать ли переход?
        /// </summary>
        protected virtual bool jump(double x, double y)
        {
            if (y >= x) return true;
            else return false;
        }

    }
}
