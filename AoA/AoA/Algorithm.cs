using System;
using IOData;

namespace AoA
{
    /// <summary>
    /// Единая оболочка для тестируемых алгоритмов
    /// </summary>
    public abstract class Algorithm
    {
        //Конструктор не может быть абстрактным
        protected Algorithm(params object[] p)
        { }

        /// <summary>
        /// Метод проводит полное обучение данного алгоритма на входных данных
        /// </summary>
        /// <param name="data">Данные обучения</param>
        public abstract void Learn(SigmentData data);

        public virtual void Learn(SigmentData data, double[] rating)
        {
            throw new NotImplementedException();
        }

        public abstract Results Calc(SigmentInputData data);

        /// <summary>
        /// Метод варьирующий порог для алгоритма
        /// </summary>
        /// <param name="th">Новый порог</param>
        public abstract void ChangeThreshold(double th);

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        { }
    }
}