using System;
using IOData;
using Metaheuristics;

namespace AoA
{
    public class RandomFindAlgorithm : RandomFinder<CVlog>
    {
        readonly Func<object[], Algorithm> getAlg;
        readonly Action<double> f = (x) => { };

        readonly FullData TestData;

        readonly SigmentData[] TestDataSigmentLearn;
        readonly SigmentData[] TestDataSigmentControl;

        const int maxStep = 1;

        public RandomFindAlgorithm(Parameter[] p, Action<int, int> w,Func<object[], Algorithm> ga, FullData td, SigmentData[] dsl, SigmentData[] dsc)
            : base(p, w, maxStep)
        {
            getAlg = ga;

            TestData = td;

            TestDataSigmentLearn = dsl;
            TestDataSigmentControl = dsc;
        }

        public RandomFindAlgorithm(Parameter[] p, Action<int, int> w, Type algType, FullData td, SigmentData[] dsl, SigmentData[] dsc)
            : base(p, w, maxStep)
        {
            getAlg = AlgorithmFactory.GetFactory(algType);

            TestData = td;

            TestDataSigmentLearn = dsl;
            TestDataSigmentControl = dsc;
        }

        protected override CVlog Quality(object[] x)
        {
            using (var exp = new Experiments(() => getAlg(x), 150))
                return exp.Run(TestData, f, TestDataSigmentLearn, TestDataSigmentControl);
        }
    }
}
