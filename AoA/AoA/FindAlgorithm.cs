using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using Metaheuristics;
using IOData;

namespace AoA
{
    public class FindAlgorithm : AnnealingFinder<CVlog>
    {
        Func<object[], Algorithm> getAlg;
        Action<double> f = (x) => { };

        FullData TestData;

        SigmentData[] TestDataSigmentLearn;
        SigmentData[] TestDataSigmentControl;

        public FindAlgorithm(Parameter[] p, Action<int> w,Func<object[], Algorithm> ga, FullData td, SigmentData[] dsl, SigmentData[] dsc)
            : base(p, w)
        {
            getAlg = ga;

            TestData = td;

            TestDataSigmentLearn = dsl;
            TestDataSigmentControl = dsc;
        }

        protected override CVlog Quality(object[] x)
        {
            Experiments exp = new Experiments(() => getAlg(x));
            Console.WriteLine("температура: {0}", temp());
            return exp.Run(TestData, f, TestDataSigmentLearn, TestDataSigmentControl);
        }

    }
}