using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyParallel;
using IOData;
using ArrayHelper;

namespace AoA
{
    public class ExperimentWorker : ParallelWorkerWithProgress
    {
        FullData data;
        Func<Algorithm> getAlgorithms;

        SigmentData[] learnDate;
        SigmentData[] controlDate;

        public Info[] info;
        
        public ROC[] rocs;

        public ExperimentWorker(int threadCount, int nn, Info[] inf, Action<double> ff)
            : base(threadCount, nn, @"ExperimentWorker№", ff)
        {
            info = inf;
        }

        public void Run(FullData fd, Func<Algorithm> algs, SigmentData[] dl, SigmentData[] dc, int ROCn)
        {
            data = fd;
            getAlgorithms = algs;
            learnDate = dl;
            controlDate = dc;

            if (ROCn == 0 || controlDate[0].Length < 2 || controlDate[0].GetResults().MaxNumber > 2) rocs = null;
            else
            {
                rocs = new ROC[ROCn];
                rocs.Done();
            }

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
                alg.Learn(learnDate[i]);

                Results CalcedLearn = alg.Calc(learnDate[i]);

                Results CalcedControl = alg.Calc(controlDate[i]);

                learnDate[i].AddLearnError(CalcedLearn, info);
                controlDate[i].AddControlError(CalcedControl, info);

                double th = -1;
                if (rocs!=null)
                for (int k = 0; k < rocs.Length; k++)
                {
                    alg.ChangeThreshold(th);
                    CalcedControl = alg.Calc(controlDate[i]);

                    int fpr = 0, tpr = 0;
                    for (int j = 0; j < controlDate[i].Length; j++)
                    {
                        if (controlDate[i].GetResults()[j].Number == 1)
                        {
                            if (CalcedControl[j].Number == 0) fpr++;
                        }
                        else
                        {
                            if (CalcedControl[j].Number == 0) tpr++;
                        }
                    }

                    rocs[k].FPR.Add((double)fpr / controlDate[i].GetResults().Counts[1]);
                    rocs[k].TPR.Add((double)tpr / controlDate[i].GetResults().Counts[0]);

                    th += 2.0 / rocs.Length;
                }
                progr((double)(i - start + 1) / (finish - start));
            }
            alg.Dispose();
        }
    }
}