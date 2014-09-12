 using System;
using VectorSpace;
using MyParallel;
using IOData;
using ArrayHelper;

namespace AoA
{
    class ExperimentWorkerOld : ParallelWorkerWithProgress
    {
        FullData data;
        Func<Algorithm> getAlgorithms;

        int[][] learnDate;
        int[][] controlDate;

        public Info[] info;
        
        public double[] foundThreshold;
        
        public ROC[] rocs;

        public ExperimentWorkerOld(int threadCount, int nn, Info[] inf, Action<double> ff)
            : base(threadCount, nn, @"ExperimentWorker№", ff)
        {
            info = inf;
            foundThreshold = new double[nn];
        }

        public void Run(FullData fd, Func<Algorithm> algs, int[][] dl, int[][] dc, int ROCn)
        {
            data = fd;
            getAlgorithms = algs;
            learnDate = dl;
            controlDate = dc;
            rocs = new ROC[ROCn];
            rocs.done();

            Run(100, 1000);
        }
        
        override protected void DoFromTo(int start, int finish, Action<double> progr)
        {
            Algorithm alg;
            lock (getAlgorithms)
            {
                alg = getAlgorithms();
            }
            for (int i = start; i < finish; i++)
            {
                SigmentData learnSigmentData = new SigmentData(data, learnDate[i]);
                SigmentData controlSigmentData = new SigmentData(data, controlDate[i]);

                alg.Learn(learnSigmentData);

                foundThreshold[i] = alg.GetThreshold();

                Results CalcedLearn = alg.Calc(learnSigmentData);

                Results CalcedControl = alg.Calc(controlSigmentData);

                learnSigmentData.AddLearnError(CalcedLearn, info);
                controlSigmentData.AddControlError(CalcedControl, info);

                double th = -1;

                for (int k = 0; k < rocs.Length; k++)
                {
                    alg.ChangeThreshold(th);
                    CalcedControl = alg.Calc(controlSigmentData);

                    int fpr = 0, tpr = 0;
                    for (int j = 0; j < controlSigmentData.Length; j++)
                    {
                        if (controlSigmentData.GetResults()[j].Number == 1)
                        {
                            if (CalcedControl[j].Number == 0) fpr++;
                        }
                        else
                        {
                            if (CalcedControl[j].Number == 0) tpr++;
                        }
                    }

                    rocs[k].FPR.Add((double)fpr / controlSigmentData.GetResults().Counts[1]);
                    rocs[k].TPR.Add((double)tpr / controlSigmentData.GetResults().Counts[0]);

                    th += 2.0 / rocs.Length;
                }
                progr((double)(i - start + 1) / (finish - start));
            }
            alg.Dispose();
        }
    }
}