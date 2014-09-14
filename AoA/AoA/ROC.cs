using System.Collections.Concurrent;
using System;

namespace AoA
{

    /// <summary>
    /// Одна точка для рок кривой
    /// </summary>
    [Serializable]
    public class ROC
    {
        public BlockingCollection<double> FPR = new BlockingCollection<double>();
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
    }
}
