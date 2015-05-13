using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyParallel;
using IOData;

namespace AoA
{
    public class RegressionExperimentWorker : ParallelWorkerWithProgress
    {
        Func<Algorithm> getAlgorithms;

        SigmentData[] learnDate;
        SigmentData[] controlDate;

        public Info[] info;

        public RegressionExperimentWorker(int threadCount, int nn, Info[] inf, Action<double> ff)
            : base(threadCount, nn, @"RegressionExperimentWorker№", ff)
        {
            info = inf;
        }

        public void Run(Func<Algorithm> algs, SigmentData[] dl, SigmentData[] dc)
        {
            getAlgorithms = algs;
            learnDate = dl;
            controlDate = dc;

            Run(100, 1000);
        }

        protected override void DoFromTo(int start, int finish, Action<double> progr)
        {
            var alg = getAlgorithms();
            for (int i = start; i < finish; i++)
            {
                alg.Learn(learnDate[i]);

                Double[] CalcedLearn = (Double[])alg.Calc(learnDate[i]);

                Double[] CalcedControl = (Double[])alg.Calc(controlDate[i]);

                learnDate[i].AddLearnError(CalcedLearn, info);
                controlDate[i].AddControlError(CalcedControl, info);

                progr((double)(i - start + 1) / (finish - start));
            }
            alg.Dispose();
        }
    }
}
