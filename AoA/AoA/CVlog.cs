using IOData;
using System;
using System.IO;
using Metaheuristics;

namespace AoA
{
    [Serializable]
    public struct CVlog : IQuality<CVlog>
    {
        public string name;

        public double avgErrorAtControl;
        public double errorOfAvgErrorAtControl;
        public double avgErrorAtTest;
        public double errorDispAtControl;
        public double errorDispAtTest;
        public double avgOverLearning;
        public double overLearningDisp;
        public double errorOfAvgOverLearning;

        public double[] avgErrorAtControls;
        public double[] avgErrorAtTests;
        public double[] errorDispAtControls;
        public double[] errorDispAtTests;
        public double[] avgOverLearnings;
        public double[] overLearningDisps;

        public double[] errorOfErrorAtControls;
        public double[] errorOfErrorAtTests;
        public double[] errorOfOverLearnings;

        public double pOfHard;

        public ROC[] rocs;

        public double AUC;
        public double errorOfAUC;

        public static double Compare(CVlog log1, CVlog log2)
        {
            double t = Statist.ExactDifference(log1.AUC, log1.errorOfAUC, log2.AUC, log2.errorOfAUC);
            if (t != 0.0) return t;
            //t = log2.pOfHard - log1.pOfHard;
            //if (t != 0.0) return t;
            t = Statist.ExactDifference(log2.avgErrorAtControl, log2.errorOfAvgErrorAtControl, log1.avgErrorAtControl, log1.errorOfAvgErrorAtControl);
            if (t != 0.0) return t;
            t = Statist.ExactDifference(log2.avgOverLearning, log2.errorOfAvgOverLearning, log1.avgOverLearning, log1.errorOfAvgOverLearning);
            if (t != 0.0) return t;

            return 0.0;
        }

        public double CompareTo(CVlog log2)
        {
            double t = Statist.ExactDifference(AUC, errorOfAUC, log2.AUC, log2.errorOfAUC);
            if (t != 0.0) return t;

            t = Statist.ExactDifference(log2.avgErrorAtControl, log2.errorOfAvgErrorAtControl, avgErrorAtControl, errorOfAvgErrorAtControl);
            if (t != 0.0) return t;
            t = Statist.ExactDifference(log2.avgOverLearning, log2.errorOfAvgOverLearning, avgOverLearning, errorOfAvgOverLearning);
            if (t != 0.0) return t;

            return 0.0;
        }

        public bool Save(Stream s)
        {
            try
            {
                //Don't work
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
