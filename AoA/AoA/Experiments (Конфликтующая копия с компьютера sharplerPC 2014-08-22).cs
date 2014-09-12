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
        const int m = 50;
        //Какая часть пойдет на контрольное множество от всех данных
        const double part = 0.25;
        //Точек для ROC кривых
        const int ROCn = 150;
        //Инициализируем константой для того что бы результат вычислений был повторим
        Random r = new Random(271828314);
        
        //информация о том, какова была величина ошибки для каждого элемента, когда он был в контрольном и когда в тестовом множества
        public Info[] info;

        Func<Algorithm> getAlgorithm;

        ExperimentWorker worker;

        #region Информация для отчета
        public double avgErrorAtControl;
        double avgErrorAtTest;
        double errorOfAvgErrorAtControl;
        double errorDispAtControl;
        double errorDispAtTest;
        double avgOverLearning;
        double overLearningDisp;
        double errorOfAvgOverLearning;
        double[] foundThreshold = new double[m];
        double avgThreshold;
        double dispThreshold;

        double[] avgErrorAtControls;
        double[] avgErrorAtTests;
        double[] errorDispAtControls;
        double[] errorDispAtTests;
        double[] avgOverLearnings;
        double[] overLearningDisps;

        double[] errorOfErrorAtControls;
        double[] errorOfErrorAtTests;
        double[] errorOfOverLearnings;

        ROC[] rocs;
        double AUC;
        double errorOfAUC;

        int countOfHard;
        public double pOfHard;
        //SpaceInfo pinfo, ninfo; // Не уверен в необходимости
        #endregion

        public Experiments(int n, Func<Algorithm> getAlg)
        {
            getAlgorithm = getAlg;
            info = new Info[n];
            info.done();

            int k = (int)Math.Round(n*part);
            testDate = new int[n - k];
            controlDate = new int[k];

            for (int i = 0; i < n-k; i++)
            {
                testDate[i]=i;
            }

            for (int i = n - k; i < n; i++)
            {
                controlDate[i-n+k] = i;
            }
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

        /*
        /// <summary>
        /// Запуск тестирования
        /// </summary>
        /// <param name="di">Входные данные</param>
        /// <param name="doo">Соответствующие им выходные "ожидаемые" значения</param>
        public CVlog Run(Vector[] di, Vector[] doo, Action<double> f)
        {
            int i;
            dateIn = di;
            dateOut = doo;

            if ((dateIn == null) || (dateIn.Length == 0) || (dateOut == null) || (dateOut.Length == 0)) throw new ArgumentNullException();

            dem = dateOut[0].Length;
            
            for (i = 0; i < info.Length; i++)
            {
                int ncl = 0;
                int p = 1;
                for (int j = 0; j < dem; j++)
                {
                    ncl += p * (int)((dateOut[i][j] + 1.0) / 2.0);
                    p *= 2;
                }
                info[i].nClass = ncl;
            }

            //Статистическая информация по исходным данным
            List<Vector> plv = new List<Vector>();
            List<Vector> nlv = new List<Vector>();
            for (i = 0; i < info.Length; i++)
                if (info[i].nClass == 0) nlv.Add(dateIn[i]);
                else plv.Add(dateIn[i]);

            Vector[] pv = plv.ToArray();
            Vector[] nv = nlv.ToArray();
            pinfo = new SpaceInfo(pv);
            ninfo = new SpaceInfo(nv);
            //

            i = 0;

            int[][] allTestDate = new int[m][];
            int[][] allControlDate = new int[m][];

            while (i < m)
            {
                Next();
                //Vector[] li = new Vector[testDate.Length];
                //Vector[] lo = new Vector[testDate.Length];
                Vector[] ci = new Vector[controlDate.Length];
                Vector[] co = new Vector[controlDate.Length];
                //for (int j = 0; j < testDate.Length; j++)
                //{
                //    li[j] = dateIn[testDate[j]];
                //    lo[j] = dateOut[testDate[j]];
                //}

                for (int j = 0; j < controlDate.Length; j++)
                {
                    ci[j] = dateIn[controlDate[j]];
                    co[j] = dateOut[controlDate[j]];
                }

                int ncount = 0, pcount = 0;

                for (int j = 0; j < controlDate.Length; j++)
                {
                    if (co[j][0] < 0)
                        ncount++;
                    else
                        pcount++;
                }

                if ((double)Math.Max(pcount, ncount) / Math.Min(pcount, ncount) > 1.5) continue;
                allTestDate[i] = testDate.CloneOk<int[]>();
                allControlDate[i] = controlDate.CloneOk<int[]>();
                i++;
            }
            
            worker = new ExperimentWorker(Environment.ProcessorCount, m, info, f);
            worker.Run(dateIn, dateOut, getAlgorithm, allTestDate, allControlDate, ROCn);

            info = worker.info;
            rocs = worker.rocs;
            foundThreshold = worker.foundThreshold;
            worker.Dispose();
            return CalcTotalInfo();
        }*/

        FullData data;

        /// <summary>
        /// Запуск тестирования
        /// </summary>
        /// <param name="FullData">Данные</param>
        public CVlog Run(FullData d, Action<double> f)
        {
            int i;
            data = d;

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

            worker = new ExperimentWorker(/*1*/Environment.ProcessorCount, m, info, f);
            worker.Run(data, getAlgorithm, allLearnDate, allControlDate, ROCn);

            rocs = worker.rocs;
            foundThreshold = worker.foundThreshold;
            worker.Dispose();
            return CalcTotalInfo();
        }

        /*
        public CVlog Run(Vector[] di, Vector[] doo, Action<double> f, double[] rating)
        {
            int i;
            dateIn = di;
            dateOut = doo;

            if ((dateIn == null) || (dateIn.Length == 0) || (dateOut == null) || (dateOut.Length == 0)) throw new ArgumentNullException();

            dem = dateOut[0].Length;

            for (i = 0; i < info.Length; i++)
            {
                int ncl = 0;
                int p = 1;
                for (int j = 0; j < dem; j++)
                {
                    ncl += p * (int)((dateOut[i][j] + 1.0) / 2.0);
                    p *= 2;
                }
                info[i].nClass = ncl;
            }

            //Статистическая информация по исходным данным
            List<Vector> plv = new List<Vector>();
            List<Vector> nlv = new List<Vector>();
            for (i = 0; i < info.Length; i++)
                if (info[i].nClass == 0) nlv.Add(dateIn[i]);
                else plv.Add(dateIn[i]);

            Vector[] pv = plv.ToArray();
            Vector[] nv = nlv.ToArray();
            pinfo = new SpaceInfo(pv);
            ninfo = new SpaceInfo(nv);
            //

            i = 0;

            int[][] allLearnDate = new int[m][];
            int[][] allControlDate = new int[m][];

            while (i < m)
            {
                Next();
                Vector[] li = new Vector[testDate.Length];
                Vector[] lo = new Vector[testDate.Length];
                Vector[] ci = new Vector[controlDate.Length];
                Vector[] co = new Vector[controlDate.Length];
                for (int j = 0; j < testDate.Length; j++)
                {
                    li[j] = dateIn[testDate[j]];
                    lo[j] = dateOut[testDate[j]];
                }

                for (int j = 0; j < controlDate.Length; j++)
                {
                    ci[j] = dateIn[controlDate[j]];
                    co[j] = dateOut[controlDate[j]];
                }

                int ncount = 0, pcount = 0;

                for (int j = 0; j < controlDate.Length; j++)
                {
                    if (co[j][0] < 0)
                        ncount++;
                    else
                        pcount++;
                }

                if ((double)Math.Max(pcount, ncount) / Math.Min(pcount, ncount) > 1.5) continue;
                allLearnDate[i] = testDate.CloneOk<int[]>();
                allControlDate[i] = controlDate.CloneOk<int[]>();
                i++;
            }

            worker = new ExperimentWorker(Environment.ProcessorCount, m, info, f);
            worker.Run(dateIn, dateOut, getAlgorithm, allLearnDate, allControlDate, ROCn, rating);

            info = worker.info;
            rocs = worker.rocs;
            foundThreshold = worker.foundThreshold;
            worker.Dispose();
            return CalcTotalInfo();
        }*/
        CVlog CalcTotalInfo()
        {
            double a = 0.95;
            avgErrorAtControl = 0.0;
            avgErrorAtTest = 0.0;
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
                avgErrorAtTest += info[i].avgErrorLearn;
                avgOverLearning += info[i].avgErrorControl - info[i].avgErrorLearn;
            }

            avgErrorAtControl /= info.Length;
            avgErrorAtTest /= info.Length;
            avgOverLearning /= info.Length;

            //дисперсии
            errorDispAtControl = 0.0;
            errorDispAtTest = 0.0;
            overLearningDisp = 0.0;
            double t;

            for (int i = 0; i < info.Length; i++)
            {
                t = info[i].avgErrorControl - avgErrorAtControl;
                errorDispAtControl += t * t;
                t = info[i].avgErrorLearn - avgErrorAtTest;
                errorDispAtTest += t * t;
                t = info[i].avgErrorControl - info[i].avgErrorLearn - avgOverLearning;
                overLearningDisp += t * t;
            }

            errorDispAtControl /= info.Length - 1;
            errorDispAtTest /= info.Length - 1;
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
            avgErrorAtTests = new double[nnn];
            errorDispAtControls = new double[nnn];
            errorDispAtTests = new double[nnn];
            avgOverLearnings = new double[nnn];
            overLearningDisps = new double[nnn];

            errorOfErrorAtControls = new double[nnn];
            errorOfErrorAtTests = new double[nnn];
            errorOfOverLearnings = new double[nnn];

            for (int i = 0; i < nnn; i++)
            {
                avgErrorAtControls[i] = 0.0;
                avgErrorAtTests[i] = 0.0;
                avgOverLearnings[i] = 0.0;

                errorOfErrorAtControls[i] = 0.0;
                errorOfErrorAtTests[i] = 0.0;
                
                int l = 0;

                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j].nClass == i)
                    {
                        l++;
                        avgErrorAtControls[i] += info[j].avgErrorControl;
                        avgErrorAtTests[i] += info[j].avgErrorLearn;
                        avgOverLearnings[i] += info[j].avgErrorControl - info[j].avgErrorLearn;

                        errorOfErrorAtControls[i] += info[j].errorOfErrorControl * info[j].errorOfErrorControl;
                        errorOfErrorAtTests[i] += info[j].errorOfErrorLearn * info[j].errorOfErrorLearn;
                    }
                }

                avgErrorAtControls[i] /= l;
                avgErrorAtTests[i] /= l;
                avgOverLearnings[i] /= l;

                errorOfErrorAtControls[i] = Math.Sqrt(errorOfErrorAtControls[i])/l;
                errorOfErrorAtTests[i] = Math.Sqrt(errorOfErrorAtTests[i]) / l;

                //дисперсии
                errorDispAtControls[i] = 0.0;
                errorDispAtTests[i] = 0.0;
                overLearningDisps[i] = 0.0;

                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j].nClass == i)
                    {
                        t = info[j].avgErrorControl - avgErrorAtControls[i];
                        errorDispAtControls[i] += t * t;
                        t = info[j].avgErrorLearn - avgErrorAtTests[i];
                        errorDispAtTests[i] += t * t;
                        t = info[j].avgErrorControl - info[j].avgErrorLearn - avgOverLearnings[i];
                        overLearningDisps[i] += t * t;
                    }
                }

                errorDispAtControls[i] /= l-1;
                errorDispAtTests[i] /= l-1;
                overLearningDisps[i] /= l-1;

                errorOfOverLearnings[i] = Statist.CalcError(l, overLearningDisps[i], a);
            }

            errorOfAvgErrorAtControl = 0.0;
            for (int i = 0; i < info.Length; i++)
                errorOfAvgErrorAtControl += info[i].errorOfErrorControl * info[i].errorOfErrorControl;
            errorOfAvgErrorAtControl = Math.Sqrt(errorOfAvgErrorAtControl) / info.Length;

            countOfHard = 0;

            //Раньше тут было >=1 но теперь вроде как надо писать так
            for (int i = 0; i < info.Length; i++)
                if (info[i].avgErrorControl - info[i].errorOfErrorControl >= 0.5)
                    countOfHard++;
            pOfHard = (double) countOfHard / info.Length;

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
            log.avgErrorAtTest = avgErrorAtTest;
            log.errorDispAtControl = errorDispAtControl;
            log.errorDispAtTest = errorDispAtTest;
            log.avgOverLearning = avgOverLearning;
            log.overLearningDisp = overLearningDisp;
            log.errorOfAvgOverLearning = errorOfAvgOverLearning;

            log.avgErrorAtControls = avgErrorAtControls.CloneOk<double[]>();
            log.avgErrorAtTests = avgErrorAtTests.CloneOk<double[]>();
            log.errorDispAtControls = errorDispAtControls.CloneOk<double[]>();
            log.errorDispAtTests = errorDispAtTests.CloneOk<double[]>();
            log.avgOverLearnings = avgOverLearnings.CloneOk<double[]>();
            log.overLearningDisps = overLearningDisps.CloneOk<double[]>();

            log.errorOfErrorAtControls = errorOfErrorAtControls.CloneOk<double[]>();
            log.errorOfErrorAtTests = errorOfErrorAtTests.CloneOk<double[]>();
            log.errorOfOverLearnings = errorOfOverLearnings.CloneOk<double[]>();

            log.pOfHard = pOfHard;

            log.rocs = rocs.CloneOk<ROC[]>();

            log.AUC = AUC;
            log.errorOfAUC = errorOfAUC;
            return log;
        }

        public bool WriteLog(string name)
        {
            StreamWriter writer = null;
            try
            {
                //Доверительный интервал
                double a = 0.95;
                double t;
                writer = new StreamWriter(name, false);
                //writer.WriteLine("Отчет об экспериментах над алгоритмом \"{0}\":", algorithm.name);
                /*
                writer.WriteLine("Информация об исходных данных:");
                writer.WriteLine("Расстояние между центрами: {0}", Math.Sqrt((double)(ninfo.Center-pinfo.Center)));
                writer.WriteLine("Отрицательные примеры:");
                writer.WriteLine("Центр: " + ninfo.Center.ToString());
                writer.WriteLine("Максимальное расстояние от центра: {0}", ninfo.MaxDistanceOfCenter);
                writer.WriteLine("Среднее расстояние от центра: {0}", ninfo.AvgDistanceOfCenter);
                writer.WriteLine("Положительные примеры:");
                writer.WriteLine("Центр: " + pinfo.Center.ToString());
                writer.WriteLine("Максимальное расстояние от центра: {0}", pinfo.MaxDistanceOfCenter);
                writer.WriteLine("Среднее расстояние от центра: {0}", pinfo.AvgDistanceOfCenter);
                */

                t = 0.0;
                for (int i = 0; i < info.Length; i++)
                    t += info[i].errorOfErrorControl * info[i].errorOfErrorControl;
                t = Math.Sqrt(t) / info.Length;
                writer.WriteLine("Средняя ошибка (на контрольном множестве) для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgErrorAtControl, errorDispAtControl, t);

                t = 0.0;
                for (int i = 0; i < info.Length; i++)
                    t += info[i].errorOfErrorLearn * info[i].errorOfErrorLearn;
                t = Math.Sqrt(t) / info.Length;
                writer.WriteLine("Средняя ошибка (на множестве обучения) для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgErrorAtTest, errorDispAtTest, t);

                writer.WriteLine("Средняя переобученность для всех объектов: {0}. Дисперсия: {1}. Отклонение: {2}", avgOverLearning, overLearningDisp, errorOfAvgOverLearning);
                writer.WriteLine();
                for (int i = 0; i < errorDispAtControls.Length; i++)
                {
                    writer.WriteLine("Средняя ошибка (на контрольном множестве) для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgErrorAtControls[i], errorDispAtControls[i], i, errorOfErrorAtControls[i]);
                    writer.WriteLine("Средняя ошибка (на множестве обучения) для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgErrorAtTests[i], errorDispAtTests[i], i, errorOfErrorAtTests[i]);
                    writer.WriteLine("Средняя переобученность для класса {2}: {0}. Дисперсия: {1}. Отклонение: {3}", avgOverLearnings[i], overLearningDisps[i], i, errorOfOverLearnings[i]);
                    writer.WriteLine();
                }
                writer.WriteLine("Cписок трудных объектов (вбросов с точки зрения алгоритма):");

                for (int i = 0; i < info.Length; i++)
                    if (info[i].avgErrorControl-info[i].errorOfErrorControl >= 0.5)
                        writer.WriteLine(i);

                writer.WriteLine("колличество трудных объектов: {0} - что составляет {1}% от общего числа.", countOfHard, pOfHard * 100.0);
                writer.WriteLine("Конец списка трудных объектв.");
                writer.WriteLine("Средний порог найденный алгоритмом: {0} и его дисперсия {1}", avgThreshold, dispThreshold);
                if (3 * Math.Sqrt(dispThreshold) >= avgThreshold+1.0) writer.WriteLine("!!! Порог для алгоритма неуслойчив");

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
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                return false;
            }
        }
    }
}