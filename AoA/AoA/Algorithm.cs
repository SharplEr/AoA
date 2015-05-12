using System;
using System.CodeDom;
using IOData;

namespace AoA
{
    /// <summary>
    /// Единая оболочка для тестируемых алгоритмов
    /// </summary>
    public abstract class Algorithm
    {
        //Должен быть задан в потомке!
        protected readonly ProblemMod mod;
        //Конструктор не может быть абстрактным
        protected Algorithm(params object[] p)
        { }

        protected Algorithm(ProblemMod m)
        {
            mod = m;
        }
        /// <summary>
        /// Метод проводит полное обучение данного алгоритма на входных данных
        /// </summary>
        /// <param name="data">Данные обучения</param>
        public abstract void Learn(SigmentData data);

        public Object Calc(SigmentInputData data)
        {
            switch (mod)
            {
                case ProblemMod.classification:
                    return Classification(data);
                case ProblemMod.regression:
                    return Regression(data);
                default:
                    throw new InvalidOperationException("mod == " + mod.ToString());
            }
        }

        protected virtual Results Classification(SigmentInputData data)
        {
            throw new NotImplementedException("Have not <Results Calc()>");
        }

        protected virtual Double[] Regression(SigmentInputData data)
        {
            throw new NotImplementedException("Have not <Double[] Calc()>");
        }

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