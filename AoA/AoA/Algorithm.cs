using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using ArrayHelper;
using IOData;

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
        public abstract void Learn(SigmentData data);

        public virtual void Learn(SigmentData data, double[] rating)
        {
            throw new NotImplementedException();
        }

        public abstract Results Calc(SigmentInputData date);

        /*
        /// <summary>
        /// Обработка данных
        /// </summary>
        /// <param name="date">Входные данные</param>
        /// <returns>Результат обработки</returns>
        public virtual Results Calc(SigmentInputData data)
        {
            Vector[] ans = new Vector[data.Length];
            for (int i = 0; i < ans.Length; i++)
                ans[i] = Calc(data[i]);
            return ans;
        }*/

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
