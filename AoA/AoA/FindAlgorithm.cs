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
    /*
    public class FindAlgorithm : AnnealingFinder<CVlog>
    {
        Func<object[], Algorithm> getAlg;
        Action<double> f;

        FullData TestData;

        SigmentData[] TestDataSigmentLearn;
        SigmentData[] TestDataSigmentControl;

        protected override CVlog Quality(object[] x)
        {
            Experiments exp = new Experiments(() => getAlg(x));
            
            return exp.Run(TestData, f, TestDataSigmentLearn, TestDataSigmentControl);
        }

    }*/
}
