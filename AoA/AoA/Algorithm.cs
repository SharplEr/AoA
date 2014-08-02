﻿using System;
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
        /// <param name="data">Данные обучения</param>
        public abstract void Learn(SigmentData data);

        public virtual void Learn(SigmentData data, double[] rating)
        {
            throw new NotImplementedException();
        }

        public abstract Results Calc(SigmentInputData date);

        /// <summary>
        /// Метод варьирующий порог для алгоритма
        /// </summary>
        /// <param name="th">Новый порог</param>
        public abstract void ChangeThreshold(double th);
       
        /// <summary>
        /// Метод возвращающий текущий порог выбранный алгоритмом
        /// </summary>
        /// <returns></returns>
        public virtual double GetThreshold()
        {
            return 0.0;
        }

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public virtual void Dispose()
        { }
    }
}
