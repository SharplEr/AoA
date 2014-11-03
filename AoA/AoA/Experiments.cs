using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using IOData;
using VectorSpace;
using System.Threading;
using MyParallel;
using ArrayHelper;

namespace AoA
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс - реализация скользящего контроля.
     * 1) Для анализа алгоритма требуется.
     *      1.1) Вызвать конструктор, для которого требуется задать общую длину всех данных.
     *      1.2) Присвоить полю algorithm оболочку для тестируемого классификатора
     *      1.3) Вызвать метод Run()
     *      1.4) Затем можно записать отчет в файл методом WriteLog()
    */
    public class Experiments
    {
        int[] testDate;
        int[] controlDate;
        //Число экспериментов (оптимальные значения 180-32000)
        int m = 1000;
        //Какая часть пойдет на контрольное множество от всех данных
        const double part = 0.25;
        //Точек для ROC кривых
        const int ROCn = 150;
        //Инициализируем константой для того что бы результат вычислений был повторим
        Random r = new Random(271828314);
        
        //информация о том, какова была величина ошибки для каждого элемента, когда он был в контрольном и когда в тестовом множества
        public Info[] info;

        Func<Algorithm> getAlgorithm;

        #region Информация для отчета
        public double avgErrorAtControl;
        double avgErrorAtLearn;
        double errorOfAvgErrorAtControl;
        double errorOfAvgErrorAtLearn;
        double errorDispAtControl;
        double errorDispAtLearn;
        double avgOverLearning;
        double overLearningDisp;
        double errorOfAvgOverLearning;
        
        double[] foundThreshold;
        double avgThreshold;
        double dispThreshold;

        double[] avgErrorAtControls;
        double[] avgErrorAtLearns;
        double[] errorDispAtControls;
        double[] errorDispAtLearns;
        double[] avgOverLearnings;
        double[] overLearningDisps;

        double[] errorOfErrorAtControls;
        double[] errorOfErrorAtLearns;
        double[] errorOfOverLearnings;

        ROC[] rocs;
        double AUC;
        double errorOfAUC;

        int countOfHardOver;
        int countOfHardUnder;
        public double pOfHardOver;
        public double pOfHardUnder;
        //SpaceInfo pinfo, ninfo; // Не уверен в необходимости

        string name = null;
        #endregion

        public Experiments(Func<Algorithm> getAlg, int mmm, string AlgName)
        {
            m = mmm;
            foundThreshold = new double[m];
            getAlgorithm = getAlg;
            name = AlgName;
        }

        public Experiments(Func<Algorithm> getAlg, int mmm)
        {
            m = mmm;
            foundThreshold = new double[m];
            getAlgorithm = getAlg;
        }

        public Experiments(Func<Algorithm> getAlg)
        {
            //foundThreshold = new double[m];
            getAlgorithm = getAlg;
        }

        /// <summary>
        /// Перемешивание
        /// </summary>
        void Next()
        {
            int j, t;
            for (int i = 0; i < controlDate.Length; i++)
            {
                j = r.Next(testDate.Length - 1);
                t = controlDate[i];
                controlDate[i] = testDate[j];
                testDate[j] = t;
            }
        }

        FullData data;

        /// <summary>
        /// Запуск тестирования
        /// </summary>
        /// <param name="FullData">Данные</param>
        public CVlog Run(FullData d, Action<double> f)
        {
            if(foundThreshold==null) foundThreshold = new double[m];
            int i;
            data = d;

            info = new Info[data.Length];
            info.done();

            int k = (int)Math.Round(data.Length * part);
            testDate = new int[data.Length - k];
            controlDate = new int[k];

            for (i = 0; i < data.Length - k; i++)
            {
                testDate[i] = i;
            }

            for (i = data.Length - k; i < data.Length; i++)
            {
                controlDate[i - data.Length + k] = i;
            }

            if (data == null) throw new ArgumentNullException();

            for (i = 0; i < info.Length; i++)
                info[i].nClass = data.Output[i].Number;

            i = 0;

            int[][] allLearnDate = new int[m][];
            int[][] allControlDate = new int[m][];

            while (i < m)
            {
                Next();

                SigmentData tempdata = new SigmentData(data, controlDate);

                if (tempdata.GetResults().Equable>1.5) continue;
                allLearnDate[i] = testDate.CloneOk<int[]>();
                allControlDate[i] = controlDate.CloneOk<int[]>();
                i++;
            }

            ExperimentWorkerOld worker;

            #if DEBUG
            worker = new ExperimentWorkerOld(1/*Environment.ProcessorCount*/, m, info, f);
            #else
            worker = new ExperimentWorkerOld(Environment.ProcessorCount, m, info, f);
            #endif

            worker.Run(data, getAlgorithm, allLearnDate, allControlDate, ROCn);

            rocs = worker.rocs;
            foundThreshold = worker.foundThreshold;
            worker.Dispose();
            return CalcTotalInfo();
        }

        public CVlog Run(FullData d, Action<double> f, SigmentData[] learnDate, SigmentData[] controlDate)
        {
            m = learnDate.Length;
            if (foundThreshold==null )foundThreshold = new double[m];
            int i;
            data = d;
            info = new Info[data.Length];
            info.done();

            if (data == null) throw new ArgumentNullException();

            for (i = 0; i < info.Length; i++)
                info[i].nClass = data.Output[i].Number;

            ExperimentWorker worker;
            #if DEBUG
            worker = new ExperimentWorker(1/*Environment.ProcessorCount*/, learnDate.Length, info, f);
            #else
            worker = new ExperimentWorker(Environment.ProcessorCount, learnDate.Length, info, f);
            #endif

            worker.Run(data, getAlgorithm, learnDate, controlDate, ROCn);

            rocs = worker.rocs;
            foundThreshold = worker.foundThreshold;
            worker.Dispose();
            return CalcTotalInfo();
        }

        CVlog CalcTotalInfo()
        {
            double a = 0.95;
            avgErrorAtControl = 0.0;
            avgErrorAtLearn = 0.0;
            avgOverLearning = 0.0;
            
            for (int i = 0; i < info.Length; i++)
            {
                info[i].avgErrorControl = 0.0;
                if (info[i].errorControl.Count != 0)
                {
                    foreach (double x in info[i].errorControl)
                        info[i].avgErrorControl += x;

                    info[i].avgErrorControl /= info[i].errorControl.Count;

                    info[i].errorOfErrorControl = 0.0;

                    foreach (double x in info[i].errorControl)
                        info[i].errorOfErrorControl += (x - info[i].avgErrorControl) * (x - info[i].avgErrorControl);

                    info[i].errorOfErrorControl /= info[i].errorControl.Count - 1;
                    info[i].errorOfErrorControl = Statist.CalcError(info[i].errorControl.Count, info[i].errorOfErrorControl, a);
                }

                avgErrorAtControl += info[i].avgErrorControl;

                info[i].avgErrorLearn = 0.0;

                if (info[i].errorLearn.Count != 0)
                {
                    foreach (double x in info[i].errorLearn)
                        info[i].avgErrorLearn += x;

                    info[i].avgErrorLearn /= info[i].errorLearn.Count;

                    info[i].errorOfErrorLearn = 0.0;

                    foreach (double x in info[i].errorLearn)
                        info[i].errorOfErrorLearn += (x - info[i].avgErrorLearn) * (x - info[i].avgErrorLearn);

                    info[i].errorOfErrorLearn /= info[i].errorLearn.Count - 1;
                    info[i].errorOfErrorLearn = Statist.CalcError(info[i].errorLearn.Count, info[i].errorOfErrorLearn, a);
                }
                avgErrorAtLearn += info[i].avgErrorLearn;
                avgOverLearning += info[i].avgErrorControl - info[i].avgErrorLearn;
            }

            avgErrorAtControl /= info.Length;
            avgErrorAtLearn /= info.Length;
            avgOverLearning /= info.Length;

            //дисперсии
            errorDispAtControl = 0.0;
            errorDispAtLearn = 0.0;
            overLearningDisp = 0.0;
            double t;

            for (int i = 0; i < info.Length; i++)
            {
                t = info[i].avgErrorControl - avgErrorAtControl;
                errorDispAtControl += t * t;
                t = info[i].avgErrorLearn - avgErrorAtLearn;
                errorDispAtLearn += t * t;
                t = info[i].avgErrorControl - info[i].avgErrorLearn - avgOverLearning;
                overLearningDisp += t * t;
            }

            errorDispAtControl /= info.Length - 1;
            errorDispAtLearn /= info.Length - 1;
            overLearningDisp /= info.Length - 1;

            avgThreshold = 0.0;

            for (int i = 0; i < m; i++)
                avgThreshold += foundThreshold[i];
            avgThreshold /= m;

            dispThreshold = 0.0;
            for (int i = 0; i < m; i++)
            {
                t = foundThreshold[i] - avgThreshold;
                dispThreshold += t * t;
            }

            dispThreshold /= m - 1;

            int nnn = data.Output.MaxNumber;

            //Для отдельных классов
            avgErrorAtControls = new double[nnn];
            avgErrorAtLearns = new double[nnn];
            errorDispAtControls = new double[nnn];
            errorDispAtLearns = new double[nnn];
            avgOverLearnings = new double[nnn];
            overLearningDisps = new double[nnn];

            errorOfErrorAtControls = new double[nnn];
            errorOfErrorAtLearns = new double[nnn];
            errorOfOverLearnings = new double[nnn];

            for (int i = 0; i < nnn; i++)
            {
                avgErrorAtControls[i] = 0.0;
                avgErrorAtLearns[i] = 0.0;
                avgOverLearnings[i] = 0.0;

                errorOfErrorAtControls[i] = 0.0;
                errorOfErrorAtLearns[i] = 0.0;
                
                int l = 0;

                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j].nClass == i)
                    {
                        l++;
                        avgErrorAtControls[i] += info[j].avgErrorControl;
                        avgErrorAtLearns[i] += info[j].avgErrorLearn;
                        avgOverLearnings[i] += info[j].avgErrorControl - info[j].avgErrorLearn;

                        errorOfErrorAtControls[i] += info[j].errorOfErrorControl * info[j].errorOfErrorControl;
                        errorOfErrorAtLearns[i] += info[j].errorOfErrorLearn * info[j].errorOfErrorLearn;
                    }
                }

                avgErrorAtControls[i] /= l;
                avgErrorAtLearns[i] /= l;
                avgOverLearnings[i] /= l;

                errorOfErrorAtControls[i] = Math.Sqrt(errorOfErrorAtControls[i])/l;
                errorOfErrorAtLearns[i] = Math.Sqrt(errorOfErrorAtLearns[i]) / l;

                //дисперсии
                errorDispAtControls[i] = 0.0;
                errorDispAtLearns[i] = 0.0;
                overLearningDisps[i] = 0.0;

                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j].nClass == i)
                    {
                        t = info[j].avgErrorControl - avgErrorAtControls[i];
                        errorDispAtControls[i] += t * t;
                        t = info[j].avgErrorLearn - avgErrorAtLearns[i];
                        errorDispAtLearns[i] += t * t;
                        t = info[j].avgErrorControl - info[j].avgErrorLearn - avgOverLearnings[i];
                        overLearningDisps[i] += t * t;
                    }
                }

                errorDispAtControls[i] /= l-1;
                errorDispAtLearns[i] /= l-1;
                overLearningDisps[i] /= l-1;

                errorOfOverLearnings[i] = Statist.CalcError(l, overLearningDisps[i], a);
            }

            errorOfAvgErrorAtControl = 0.0;
            for (int i = 0; i < info.Length; i++)
                errorOfAvgErrorAtControl += info[i].errorOfErrorControl * info[i].errorOfErrorControl;
            errorOfAvgErrorAtControl = Math.Sqrt(errorOfAvgErrorAtControl) / info.Length;

            errorOfAvgErrorAtLearn = 0.0;
            for (int i = 0; i < info.Length; i++)
                errorOfAvgErrorAtLearn += info[i].errorOfErrorLearn * info[i].errorOfErrorLearn;
            errorOfAvgErrorAtLearn = Math.Sqrt(errorOfAvgErrorAtLearn) / info.Length;

            countOfHardOver = 0;

            //Раньше тут было >=1 но теперь вроде как надо писать так
            for (int i = 0; i < info.Length; i++)
                if (info[i].avgErrorControl - info[i].errorOfErrorControl >= 0.5)
                    countOfHardUnder++;
            pOfHardUnder = (double)countOfHardUnder / info.Length;

            for (int i = 0; i < info.Length; i++)
                if (info[i].avgErrorControl + info[i].errorOfErrorControl >= 0.5)
                    countOfHardOver++;
            pOfHardOver = (double)countOfHardOver / info.Length;

            errorOfAvgOverLearning = Statist.CalcError(info.Length, overLearningDisp, a);
            //Для ROC кривых
            for (int i = 0; i < rocs.Length; i++)
            {
                rocs[i].refresh();
                rocs[i].avgFPR = 0.0;
                rocs[i].avgTPR = 0.0;

                for (int j = 0; j < rocs[i].FPRa.Length; j++)
                {
                    rocs[i].avgFPR += rocs[i].FPRa[j];
                }
                rocs[i].avgFPR /= rocs[i].FPRa.Length;

                for (int j = 0; j < rocs[i].TPRa.Length; j++)
                {
                    rocs[i].avgTPR += rocs[i].TPRa[j];
                }
                rocs[i].avgTPR /= rocs[i].TPRa.Length;

                rocs[i].dispFPR = 0.0;
                rocs[i].dispTPR = 0.0;

                for (int j = 0; j < rocs[i].FPRa.Length; j++)
                {
                    t = rocs[i].FPRa[j] - rocs[i].avgFPR;
                    rocs[i].dispFPR += t * t;
                }
                rocs[i].dispFPR /= rocs[i].FPRa.Length - 1;

                for (int j = 0; j < rocs[i].TPRa.Length; j++)
                {
                    t = rocs[i].TPRa[j] - rocs[i].avgTPR;
                    rocs[i].dispTPR += t * t;
                }
                rocs[i].dispTPR /= rocs[i].TPRa.Length - 1;
            }

            AUC = 0.0;

            for (int i = 1; i < rocs.Length - 1; i++)
                AUC += rocs[i].avgTPR * (rocs[i + 1].avgFPR - rocs[i - 1].avgFPR);

            AUC += rocs[0].avgTPR * (rocs[1].avgFPR - rocs[0].avgFPR) + rocs[rocs.Length - 1].avgTPR * (rocs[rocs.Length - 1].avgFPR - rocs[rocs.Length - 2].avgFPR);

            AUC *= 0.5;

            errorOfAUC = Statist.CalcQError(rocs[0].TPR.Count, rocs[0].dispTPR, a) * (rocs[1].avgFPR - rocs[0].avgFPR) * (rocs[1].avgFPR - rocs[0].avgFPR);
            errorOfAUC += Statist.CalcQError(rocs[rocs.Length - 1].TPR.Count, rocs[rocs.Length - 1].dispTPR, a) * (rocs[rocs.Length - 1].avgFPR - rocs[rocs.Length - 2].avgFPR) * (rocs[rocs.Length - 1].avgFPR - rocs[rocs.Length - 2].avgFPR);
            errorOfAUC += Statist.CalcQError(rocs[0].FPR.Count, rocs[0].dispFPR, a) * (rocs[1].avgTPR - rocs[0].avgTPR) * (rocs[1].avgTPR - rocs[0].avgTPR) + Statist.CalcQError(rocs[1].FPR.Count, rocs[1].dispFPR, a) * (rocs[2].avgTPR + rocs[0].avgTPR) * (rocs[2].avgTPR + rocs[0].avgTPR);
            errorOfAUC += Statist.CalcQError(rocs[rocs.Length - 1].FPR.Count, rocs[rocs.Length - 1].dispFPR, a) * (rocs[rocs.Length - 2].avgTPR + rocs[rocs.Length - 1].avgTPR) * (rocs[rocs.Length - 2].avgTPR + rocs[rocs.Length - 1].avgTPR) + Statist.CalcQError(rocs[rocs.Length - 2].FPR.Count, rocs[rocs.Length - 2].dispFPR, a) * (rocs[rocs.Length - 3].avgTPR - rocs[rocs.Length - 1].avgTPR) * (rocs[rocs.Length - 3].avgTPR - rocs[rocs.Length - 1].avgTPR);
            for (int i = 1; i < rocs.Length - 1; i++)
                errorOfAUC += Statist.CalcQError(rocs[i].TPR.Count, rocs[i].dispTPR, a) * (rocs[i + 1].avgFPR - rocs[i - 1].avgFPR) * (rocs[i + 1].avgFPR - rocs[i - 1].avgFPR);

            for (int i = 2; i < rocs.Length - 2; i++)
                errorOfAUC += Statist.CalcQError(rocs[i].FPR.Count, rocs[i].dispFPR, a) * (rocs[i - 1].avgTPR - rocs[i + 1].avgTPR) * (rocs[i - 1].avgTPR - rocs[i + 1].avgTPR);

            errorOfAUC = 0.5 * Math.Sqrt(errorOfAUC);

            CVlog log;
            log.avgErrorAtControl = avgErrorAtControl;
            log.errorOfAvgErrorAtControl = errorOfAvgErrorAtControl;
            log.errorOfAvgErrorAtLearn = errorOfAvgErrorAtLearn;
            log.avgErrorAtLearn = avgErrorAtLearn;
            log.errorDispAtControl = errorDispAtControl;
            log.errorDispAtLearn = errorDispAtLearn;
            log.avgOverLearning = avgOverLearning;
            log.overLearningDisp = overLearningDisp;
            log.errorOfAvgOverLearning = errorOfAvgOverLearning;

            log.avgErrorAtControls = avgErrorAtControls.CloneOk<double[]>();
            log.avgErrorAtLearns = avgErrorAtLearns.CloneOk<double[]>();
            log.errorDispAtControls = errorDispAtControls.CloneOk<double[]>();
            log.errorDispAtLearns = errorDispAtLearns.CloneOk<double[]>();
            log.avgOverLearnings = avgOverLearnings.CloneOk<double[]>();
            log.overLearningDisps = overLearningDisps.CloneOk<double[]>();

            log.errorOfErrorAtControls = errorOfErrorAtControls.CloneOk<double[]>();
            log.errorOfErrorAtLearns = errorOfErrorAtLearns.CloneOk<double[]>();
            log.errorOfOverLearnings = errorOfOverLearnings.CloneOk<double[]>();

            log.pOfHardOver = pOfHardOver;
            log.pOfHardUnder = pOfHardUnder;

            log.rocs = rocs.CloneOk<ROC[]>();

            log.AUC = AUC;
            log.errorOfAUC = errorOfAUC;
            log.name = name;
            
            return log;
        }

    }
}