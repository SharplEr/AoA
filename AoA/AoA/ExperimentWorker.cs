 using System;
using VectorSpace;
using MyParallel;
using IOData;

namespace AoA
{
    class ExperimentWorker : ParallelWorkerWithProgress
    {
        Vector[] dateIn;
        Vector[] dateOut;
        Func<Algorithm> getAlgorithms;

        int[][] learnDate;
        int[][] controlDate;

        public Info[] info;
        
        public double[] foundThreshold;
        
        public ROC[] rocs;

        public ExperimentWorker(int threadCount, int nn, Info[] inf, Action<double> ff)
            : base(threadCount, nn, @"ExperimentWorker№", ff)
        {
            info = inf;
            foundThreshold = new double[nn];
        }

        public void Run(Vector[] di, Vector[] doo, Func<Algorithm> algs, int[][] dl, int[][] dc, int ROCn)
        {
            dateIn = di;
            dateOut = doo;
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
                Vector[] learnInput = new Vector[learnDate[i].Length];
                Vector[] learnOutput = new Vector[learnDate[i].Length];
                Vector[] controlInput = new Vector[controlDate[i].Length];
                Vector[] controlOutput = new Vector[controlDate[i].Length];
                for (int j = 0; j < learnDate[i].Length; j++)
                {
                    learnInput[j] = dateIn[learnDate[i][j]];
                    learnOutput[j] = dateOut[learnDate[i][j]];
                }

                for (int j = 0; j < controlDate[i].Length; j++)
                {
                    controlInput[j] = dateIn[controlDate[i][j]];
                    controlOutput[j] = dateOut[controlDate[i][j]];
                }

                int ncount = 0, pcount = 0;

                for (int j = 0; j < controlDate[i].Length; j++)
                {
                    if (controlOutput[j][0] < 0)
                        ncount++;
                    else
                        pcount++;
                }

                alg.Learn(learnInput.CloneOk(), learnOutput.CloneOk());

                foundThreshold[i] = alg.GetThreshold();

                Vector[] CalcedLearn = alg.Calc(learnInput.CloneOk());

                for (int j = 0; j < learnDate[i].Length; j++)
                {
                    info[learnDate[i][j]].errorLearn.Add(Math.Sqrt((double)(learnOutput[j] - CalcedLearn[j])));
                }

                Vector[] CalcedControl = alg.Calc(controlInput.CloneOk());
                for (int j = 0; j < controlDate[i].Length; j++)
                {
                    info[controlDate[i][j]].errorControl.Add(Math.Sqrt((double)(controlOutput[j] - CalcedControl[j])));
                }

                double th = -1;

                for (int k = 0; k < rocs.Length; k++)
                {
                    alg.ChangeThreshold(2.5 * th);
                    CalcedControl = alg.Calc(controlInput.CloneOk());

                    int fpr = 0, tpr = 0;
                    for (int j = 0; j < controlDate[i].Length; j++)
                    {
                        if (controlOutput[j][0] < 0)
                        {
                            if (CalcedControl[j][0] > 0) fpr++;
                        }
                        else
                        {
                            if (CalcedControl[j][0] > 0) tpr++;
                        }
                    }

                    rocs[k].FPR.Add((double)fpr / ncount);
                    rocs[k].TPR.Add((double)tpr / pcount);

                    th += 2.0 / rocs.Length;
                }
                progr((double)(i - start + 1) / (finish - start));
            }
            alg.Dispose();
        }
    }
}