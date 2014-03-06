using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;

namespace AoA
{
    /// <summary>
    /// Единая оболочка для тестируемых алгоритмов
    /// </summary>
    public abstract class Algorithm
    {
        /// <summary>
        /// Имя алгоритма
        /// </summary>
        public string name;

        /// <summary>
        /// Метод проводит полное обучение данного алгоритма на входных данных
        /// </summary>
        /// <param name="inputDate">Входные данные обучения</param>
        /// <param name="resultDate">Ожидаемые ответы</param>
        public abstract void Learn(Vector[] inputDate, Vector[] resultDate);

        /// <summary>
        /// Обработка данных
        /// </summary>
        /// <param name="date">Входные данные</param>
        /// <returns>Результат обработки</returns>
        public abstract Vector[] Calc(Vector[] date);

        /// <summary>
        /// Метод варьирующий порог для алгоритма
        /// </summary>
        /// <param name="th">Новый порог</param>
        public abstract void ChangeThreshold(double th);
       
        /// <summary>
        /// Метод возвращающий текущий порог выбранный алгоритмом
        /// </summary>
        /// <returns></returns>
        public abstract double GetThreshold();


        public virtual void Dispose()
        { }
    }
}
