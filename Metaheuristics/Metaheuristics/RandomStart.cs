using System;

namespace Metaheuristics
{
    class RandomStart: IGoStart
    {
        readonly Random random;
        readonly Parameter[] parameters;

        public RandomStart(Random r, Parameter[] ps)
        {
            random = r;
            parameters = ps;
        }

        public void GetStart(int[] p)
        {
            for (int i = 0; i < p.Length; i++)
                p[i] = random.Next(parameters[i].min, parameters[i].max + 1);   //Верхний предел исключён.
        }
    }
}
