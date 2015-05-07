using System;
using System.Collections.Concurrent;

namespace AoA
{

    /// <summary>
    /// Одна точка для рок кривой
    /// </summary>
    [Serializable]
    public class ROC : IDisposable
    {
        [NonSerialized]
        public BlockingCollection<double> FPR = new BlockingCollection<double>();
        [NonSerialized]
        public BlockingCollection<double> TPR = new BlockingCollection<double>();
        public double[] FPRa;
        public double[] TPRa;
        public double avgFPR;
        public double avgTPR;
        public double dispFPR;
        public double dispTPR;

        public void refresh()
        {
            FPRa = FPR.ToArray();
            TPRa = TPR.ToArray();
        }

        public void Dispose()
        {
            FPR.Dispose();
            TPR.Dispose();
        }
    }
}
