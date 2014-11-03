using IOData;
using System;
using System.IO;
using Metaheuristics;
using System.Collections.Generic;

namespace AoA
{
    [Serializable]
    public struct CVlog : IQuality<CVlog>
    {
        public string name;

        public double avgErrorAtControl;
        public double errorOfAvgErrorAtControl;
        public double errorOfAvgErrorAtLearn;
        public double avgErrorAtLearn;
        public double errorDispAtControl;
        public double errorDispAtLearn;
        public double avgOverLearning;
        public double overLearningDisp;
        public double errorOfAvgOverLearning;

        public double[] avgErrorAtControls;
        public double[] avgErrorAtLearns;
        public double[] errorDispAtControls;
        public double[] errorDispAtLearns;
        public double[] avgOverLearnings;
        public double[] overLearningDisps;

        public double[] errorOfErrorAtControls;
        public double[] errorOfErrorAtLearns;
        public double[] errorOfOverLearnings;

        public double pOfHardOver;
        public double pOfHardUnder;

        public ROC[] rocs;

        public double AUC;
        public double errorOfAUC;

        public static double Compare(CVlog log1, CVlog log2)
        {
            double t = Statist.ExactDifference(log1.AUC, log1.errorOfAUC, log2.AUC, log2.errorOfAUC);
            if (t != 0.0) return t;
            t = Statist.ExactDifference(log2.avgErrorAtControl, log2.errorOfAvgErrorAtControl, log1.avgErrorAtControl, log1.errorOfAvgErrorAtControl);
            if (t != 0.0) return t/2.0;
            t = Statist.ExactDifference(log2.avgOverLearning, log2.errorOfAvgOverLearning, log1.avgOverLearning, log1.errorOfAvgOverLearning);
            if (t != 0.0) return t/4.0;

            t = log2.errorOfAUC - log1.errorOfAUC;
            return t/8.0;
        }

        public double CompareTo(CVlog log2)
        {
            double t = Statist.ExactDifference(AUC, errorOfAUC, log2.AUC, log2.errorOfAUC);
            if (t != 0.0) return t;
            t = Statist.ExactDifference(log2.avgErrorAtControl, log2.errorOfAvgErrorAtControl, avgErrorAtControl, errorOfAvgErrorAtControl);
            if (t != 0.0) return t/2.0;
            t = Statist.ExactDifference(log2.avgOverLearning, log2.errorOfAvgOverLearning, avgOverLearning, errorOfAvgOverLearning);
            if (t != 0.0) return t/4.0;

            t = log2.errorOfAUC - errorOfAUC;
            return t/8.0;
            //return log2.avgErrorAtLearn - avgErrorAtLearn;
        }

        public bool Save(StreamWriter writer)
        {
            try
            {
                writer.WriteLine("Средняя ошибка (на контрольном множестве) для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgErrorAtControl, errorDispAtControl, errorOfAvgErrorAtControl);

                writer.WriteLine("Средняя ошибка (на множестве обучения) для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgErrorAtLearn, errorDispAtLearn, errorOfAvgErrorAtLearn);

                writer.WriteLine("Средняя переобученность для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgOverLearning, overLearningDisp, errorOfAvgOverLearning);
                writer.WriteLine();
                for (int i = 0; i < errorDispAtControls.Length; i++)
                {
                    writer.WriteLine("Средняя ошибка (на контрольном множестве) для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgErrorAtControls[i], errorDispAtControls[i], i, errorOfErrorAtControls[i]);
                    writer.WriteLine("Средняя ошибка (на множестве обучения) для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgErrorAtLearns[i], errorDispAtLearns[i], i, errorOfErrorAtLearns[i]);
                    writer.WriteLine("Средняя переобученность для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgOverLearnings[i], overLearningDisps[i], i, errorOfOverLearnings[i]);
                    writer.WriteLine();
                }
                
                writer.WriteLine("Процентр трудных объектов находится в диапазоне: [{0} %; {1} %].", pOfHardUnder *100.0,pOfHardOver * 100.0);

                writer.WriteLine();

                writer.WriteLine("ROC");

                writer.WriteLine("ROC кривая для класса больных");

                writer.WriteLine("FPR:");
                for (int i = 0; i < rocs.Length; i++)
                    writer.WriteLine(rocs[i].avgFPR);

                writer.WriteLine("TPR:");
                for (int i = 0; i < rocs.Length; i++)
                    writer.WriteLine(rocs[i].avgTPR);

                writer.WriteLine("AUC: {0}", AUC);

                writer.WriteLine("Ошибка AUC: {0}", errorOfAUC);

                writer.WriteLine("Конец отчета.");
                writer.Close();
                writer.Dispose();
                return true;
            }
            catch
            {
                writer.Close();
                writer.Dispose();
                return false;
            }
        }
    }
}
