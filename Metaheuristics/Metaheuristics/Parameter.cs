using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Metaheuristics
{
    /// <summary>
    /// Класс представляющий один параметр
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public readonly string name;
        /// <summary>
        /// Максимальное значение
        /// </summary>
        public readonly int max;
        /// <summary>
        /// Минимальное значение
        /// </summary>
        public readonly int min;
        /// <summary>
        /// Преобразователь из дискретной сетки в любую другую
        /// </summary>
        public readonly Func<int, object> convert;
        /// <summary>
        /// max-min
        /// </summary>
        public readonly int length; //Постоянно нужно, так что бы не добавить?

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="mx">Максимум</param>
        /// <param name="mn">Минимум</param>
        /// <param name="n">Имя параметра</param>
        /// <param name="f">Функция преобразования</param>
        public Parameter(int mx, int mn, string n, Func<int, object> f)
        {
            max = mx;
            min = mn;
            name = n;
            convert = f;
            length = max - min;
        }
        
        public static Parameter[] LoadFromDLL(string file)
        {
            Assembly a = Assembly.LoadFrom(file);
            Parameter[] ans = null;

            foreach (Type t in a.GetExportedTypes())
            {
                if (typeof(ParametersSeter).IsAssignableFrom(t))
                {
                    ans = (Parameter[])t.GetMethod("GetParameters").Invoke(null, null);
                    break;
                }
            }

            return ans;
        }

        public static Parameter[] LoadFromDLL(string file, string name)
        {
            Assembly a = Assembly.LoadFrom(file);
            Parameter[] ans = null;

            foreach (Type t in a.GetExportedTypes())
            {
                if (typeof(ParametersSeter).IsAssignableFrom(t) && (t.Name == name))
                {
                    ans = (Parameter[])t.GetMethod("GetParameters").Invoke(null, null);
                    break;
                }
            }

            return ans;
        }
    }
}