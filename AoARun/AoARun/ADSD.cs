using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace genome_tree
{
    //Название класса объясняет причину его существования
    //Alexey Dolgih Suck Dick
    public class ADSD
    {
        public static int maincol = 0;//столбец с классами
        public static List<int> variety = new List<int>();//данные, которые нужня для алгоритмов построения.[0] - кол-во значений [1] - кол-во значений, относящиеся к класса 1 ... и т.д.
        public static int thrcount = 0;//счётчик потоков
        public static List<TREE> ltree = new List<TREE>();//список деревьев
        public static List<THREAD> lthr = new List<THREAD>();//список потоков
        public static Dictionary<int, THREAD> dthr = new Dictionary<int, THREAD>();//список потоков - новая версия 
        public static Queue<NODE> qnodes = new Queue<NODE>();//вершины, которые жду обработки
        public static bool firstthread = true;//если поток первый. остальные будут запускаться с задержкой
        //public static List<double> rowsweight = new List<double>();//вес уравнений
        //public static List<ROWSWEIGHT> lcrw = new List<ROWSWEIGHT>();
        static object LOCOBJ = new object();
        public static Dictionary<int, double> boosteqlw = new Dictionary<int, double>();

        /*
        private void standarttrees()
        {
            clearglobe();
            int randomrow = -1;
            Random random = new Random();
            cleartext();
            MAINF main = this.Owner as MAINF;

            for (int rep = 0; rep < Convert.ToUInt32(repcount.Value); rep++)
            {
                for (int iterator = 0; iterator < alg_box.Items.Count; iterator++)
                {
                    if (alg_box.GetItemChecked(iterator))
                    {
                        TREEALG tr = TREEALG.ID3;
                        switch (alg_box.Items[iterator].ToString())
                        {
                            case ("ID3"):
                                tr = config.TREEALG.ID3;
                                break;
                            case ("C4.5"):
                                tr = config.TREEALG.C4_5;
                                break;
                            case ("CART"):
                                tr = config.TREEALG.CART;
                                break;
                            case ("LISTBB"):
                                tr = config.TREEALG.LISTBB;
                                break;
                        }
                        config.SYSMES = "start create tree's";
                        config.LOG(config.LOGTYPE.ACT);
                        TREE tree = new TREE(tr);
                        tree.report.Add("#$#");
                        tree.report.Add("Начато построение дерева по алгоритму " + tr.ToString());
                        NODE pnode = new NODE(tr, tree);//создаём главную вершину. заносим в неё данные о таблице.
                        ltree.Add(tree);
                        for (int col = 0; col < main.gdata.Columns.Count; col++)
                        {
                            if (col != maincol)
                                pnode.columns.Add(col);
                        }
                        if (useadaboost.Checked)
                        {
                            if (lcrw.Count > rep)
                            {
                                for (int cr = 0; cr < lcrw[rep].rowsweight.Count; cr++)
                                {
                                    pnode.rows.Add(lcrw[rep].rowsweight.Keys.ElementAt(cr));
                                }
                                tree.rwpoint = (lcrw.Count - 1);
                            }
                            else
                            {
                                ROWSWEIGHT crw = new ROWSWEIGHT(rep);
                                lcrw.Add(crw);
                                tree.rwpoint = (lcrw.Count - 1);
                                for (int row = 0; row < Convert.ToInt32(nsubpoints.Value); row++)
                                {
                                    randomrow = random.Next(Convert.ToInt32(starteq.Value), Convert.ToInt32(fineq.Value));
                                    if (!pnode.rows.Contains(randomrow))
                                    {
                                        pnode.rows.Add(randomrow);
                                        crw.rowsweight.Add(pnode.rows[pnode.rows.Count - 1], (1 / Convert.ToDouble(nsubpoints.Value)));
                                    }
                                    else
                                        --row;
                                }
                            }
                        }
                        else
                        {
                            for (int row = Convert.ToInt32(starteq.Value); row < Convert.ToInt32(fineq.Value); row++)
                                pnode.rows.Add(row);
                        }
                        qnodes.Enqueue(pnode);  //заносим в очередь       
                        for (int c = 0; c < config.MAXTHREAD; c++)
                        {
                            THREAD thr = new THREAD(lthr.Count.ToString(), this, tr);
                            lthr.Add(thr);
                            config.SYSMES = "Create new thread " + lthr.Count.ToString();
                            config.LOG(config.LOGTYPE.ACT);
                        }
                    }
                }
            }
            firstthread = true;
        }*/

        public void srncol(ref NODE cnode, TREEALG treealg)//search new column - ищем новый столбец для разбиения//cnode - current node: экзм.класса, в котором есть список оставшихся в рассмотрении столбцов.
        {
            //double bestval = main.gdata.Rows.Count * (-1);
            double res = -2;
            int bestcol = -1;
            int missrow = 0;
            
            bool semaphore = false;
            
            for (int iterator = 0; iterator < cnode.columns.Count; ++iterator)
            {
                variety.Clear();
              //  getdata(ref cnode.class1, ref variety, cnode.columns[iterator], cnode.rows, missrow);


                if (cnode.class1 != -1 && cnode.class1 != 0)
                {
                    switch (treealg)
                    {
                        case (TREEALG.ID3):
                            res = ID3.Gain(variety, cnode.rows.Count, cnode.class1);
                            break;
                        case (TREEALG.C4_5):
                            res = C4_5.GainRation(variety, cnode.rows.Count, cnode.class1, missrow);
                            break;
                        case (TREEALG.CART):
                            res = CART.Gain(variety);
                            break;
                        case (TREEALG.LISTBB):
                            res = (-1) * LISTBB.Gain(variety);
                            break;
                    }
                    if (res > bestval)
                    { bestval = res; bestcol = cnode.columns[iterator]; }
                }
                else
                {
                    semaphore = true;
                    break;
                }
            }
        }
        /*
        private void crnnode(ref NODE parent)//создаём потомков
        {
            try
            {
                List<string> valcount = new List<string>();
                int pointer = 0;

                parent.tree.report.Add("Ищем потомков для " + parent.ID.ToString());
                for (int iterator = 0; iterator < parent.rows.Count; ++iterator)
                {
                    if (valcount.Contains(main.gdata[parent.column, parent.rows[iterator]].Value.ToString()))//если мы уже встречали такое значение
                    {
                        if (parent.tra != TREEALG.CART && parent.tra != TREEALG.LISTBB)
                        {
                            for (pointer = 0; pointer < valcount.Count; pointer += 2)
                            {
                                if (valcount[pointer] == main.gdata[parent.column, parent.rows[iterator]].Value.ToString())
                                {
                                    valcount[++pointer] += " " + parent.rows[iterator].ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (pointer = 0; pointer < valcount.Count; pointer += 3)
                            {
                                if (valcount[pointer] == main.gdata[parent.column, parent.rows[iterator]].Value.ToString())
                                {
                                    valcount[++pointer] += " " + parent.rows[iterator].ToString();
                                    valcount[++pointer] = (Convert.ToInt32(valcount[pointer]) + 1).ToString();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        valcount.Add(main.gdata[parent.column, parent.rows[iterator]].Value.ToString());
                        parent.branchval.Add(main.gdata[parent.column, parent.rows[iterator]].Value.ToString());
                        valcount.Add(parent.rows[iterator].ToString());
                        if (parent.tra == config.TREEALG.CART || parent.tra == config.TREEALG.LISTBB) valcount.Add("1");
                    }
                }
                if (parent.tra == config.TREEALG.CART || parent.tra == config.TREEALG.LISTBB)
                    FUNC.splitval(ref valcount);
                config.SYSMES = "branchval of node ID: " + parent.ID.ToString() + " :: ";
                for (int i = 0; i < parent.branchval.Count; i++)
                    config.SYSMES += " " + config.CONVTOCHAR(parent.branchval[i]);
                config.SYSMES += " method: " + parent.tra.ToString();
                config.LOG(config.LOGTYPE.ACT);
                parent.tree.report.Add("Значения ветвей вершины " + gcn(parent.column) + " ID: " + parent.ID.ToString() + " ::");
                for (int i = 0; i < parent.branchval.Count; i++)
                    parent.tree.report.Add(config.CONVTOCHAR(parent.branchval[i]));
                for (int iterator = 0; iterator < valcount.Count / 2; ++iterator)
                {
                    NODE node = new NODE(parent.tree.ID, parent.ID, parent.columns, parent.column, parent.tra, parent.tree);
                    parent.childes.Add(parent.tree.ID++);
                    filllist(ref node, ref valcount);//строчку в числа

                    parent.tree.report.Add("Новый потомок: ID = " + node.ID.ToString());
                    if (node.rows.Count < (int)min_eq.Value)
                    {
                        node.leaf = true;
                        node.class1 = classcount(main, node.rows);
                        parent.tree.nodes.Add(node);
                        parent.tree.report.Add("Потомок ID: " + node.ID + " является листом\n" + "Количество точек: " + node.rows.Count.ToString());
                    }
                    else
                        qnodes.Enqueue(node);//добавлеям в очередь для последующей обработки
                }
                parent.tree.nodes.Add(parent); //добавляем текущего родителя в список "готовых" вершин
            }
            catch (SystemException ex)
            {
                parent.tree.nodes.Add(parent);
            }
        }*/


    }

    public enum TREEALG
    {
        ID3,
        C4_5,
        CART,
        LISTBB
    }

    public class TREE
    {
        public List<NODE> nodes;
        public uint ID;
        public TREEALG algname;
        public List<int> gbrbuff;//буффер для формирования графического представления
        public List<GTREE> gtree;//список всех ветвей для графического отображения 
        public List<string> report = new List<string>();//отчёт по построению
        public bool rules = false;
        public double aplha;//коэффициент алгоритма 
        public int rwpoint;//указать на класс с весами 

        public TREE(TREEALG al)
        {
            algname = al;
            ID = 1;
            nodes = new List<NODE>();
        }

        public void sortnodes()//отсортировать nodes по ID
        {
            List<NODE> gbbr = (from element in nodes orderby element.ID select element).ToList<NODE>();
            nodes.Clear();
            for (int iterator = 0; iterator < gbbr.Count; iterator++)
            {
                nodes.Add(gbbr[iterator]);
            }
            gbbr.Clear();
        }

        public void crtgtree()
        {
            sortnodes();//сначала отсортируем список вершин по ID
            gbrbuff = new List<int>();
            gbrbuff.Add(0);
            List<int> use = new List<int>();
            use.Add(0);
            int curparent = 0;
            gtree = new List<GTREE>();
            for (int iterator = 1; iterator < nodes.Count; iterator++)
            {
                if (nodes[iterator].parent == curparent && (!use.Contains(iterator)))
                {
                    gbrbuff.Add(iterator);
                    use.Add(iterator);
                    if (nodes[iterator].leaf)
                    {
                        GTREE gt = new GTREE(gbrbuff);
                        if (nodes[iterator].rows.Count - nodes[iterator].class1 <= nodes[iterator].class1)
                            gt.cl1 = true;
                        gtree.Add(gt);
                        gbrbuff.RemoveAt(gbrbuff.Count - 1);
                        iterator = curparent;
                    }
                    else curparent = iterator;
                    continue;
                }
                if (iterator == nodes.Count - 1 && use.Count != nodes.Count)
                {
                    gbrbuff.RemoveAt(gbrbuff.Count - 1);
                    iterator = curparent = gbrbuff[gbrbuff.Count - 1];
                }
            }
        }
    }
    public class GTREE
    {
        public List<int> gbranch;
        public double probability;
        public bool cl1 = false;
        public int treeid;

        public GTREE(List<int> curbr)
        {
            gbranch = new List<int>();
            for (int i = 0; i < curbr.Count; i++)
            {
                gbranch.Add(curbr[i]);
            }
        }
    }

    public class NODE
    {
        public uint ID; //нормер, которым была определена вершина
        public int column; //столбец, значения которого будут взяты для разбиения
        public List<int> rows;  //строки, оставшиеся в рассмотрении
        public List<int> columns;  //столбцы, оставшиеся в рассмотрении
        public List<uint> childes; //дети - ID вершин-детей данной вершины
        public List<string> branchval; //значения ветвей
        public string value;//значение ветви от предка к текущей вершине
        public uint parent; //указатель на родителя
        public bool leaf; //вершина является листом
        public int class1 = -1; //счётчик класса 1 
        public TREE tree;
        public TREEALG tra;//каким методом была получена вершина tree algoritm

        public NODE(TREEALG tr, TREE prtree)
        {
            ID = 0;
            leaf = false;
            parent = 0;
            rows = new List<int>();
            columns = new List<int>();
            childes = new List<uint>();
            branchval = new List<string>();
            value = " ";
            tra = tr;
            tree = prtree;
        }

        public NODE(uint id, uint par, List<int> parcols, int parcol, TREEALG tr, TREE prtree)
        {
            ID = id;
            parent = par;
            rows = new List<int>();
            columns = new List<int>();
            childes = new List<uint>();
            branchval = new List<string>();
            tree = prtree;
            for (int iterator = 0; iterator < parcols.Count; iterator++)
            {
                if (parcols[iterator] == parcol) continue;
                columns.Add(parcols[iterator]);
            }
            tra = tr;
        }
    }

    public class THREAD
    {
        Thread thr;
        byte tryes = 0;
        ADSD ftr;
        bool adaboost = false;
        TREEALG alg;
        int pointer;
        int thrid;
        int strpos;
        int fpos;

        public static bool tflag = false;

        public THREAD(string name, ADSD cftree, TREEALG useal)
        {
            
            thr = new Thread(thrmcrtree);
            thr.Name = name;
            ftr = cftree;
            if (!ADSD.firstthread)
                Thread.Sleep(3*1000);
            else ADSD.firstthread = false;
            alg = useal;
            
            thr.Start();
        }

        public THREAD(ADSD cftree, bool useboost)
        {

            thr = new Thread(thrmcrgtree);
           // thr.IsBackground = true;
            ftr = cftree;
            adaboost = useboost;

            thr.Start();
        }

        public THREAD(ADSD ft, int point)
        {
            thr = new Thread(crtr);
            thr.Name = point.ToString();
            ftr = ft;
            thr.Priority = ThreadPriority.Highest;
            pointer = point;
            thrid = point;
            ADSD.lthr.Add(this);
            thr.Start();
        }

        public THREAD(ADSD ftr, int startpos, int finpos)
        {
            thrid = ADSD.dthr.Keys.Count;
            ADSD.dthr.Add(ADSD.dthr.Keys.Count, this);
            tflag = false;
            thr = new Thread(thrmcpr);
            thr.Priority = ThreadPriority.AboveNormal;
            strpos = startpos;
            fpos = finpos;
            this.ftr = ftr;
            thr.Start();
        }

        void thrmcpr()//manage calc probability
        {
            List<int> crows = new List<int>();

            for (int st = strpos; st < fpos; st++)
            {
                for (int r = 0; r < (BaeysTREE.rules[st].gbranch.Count-1); r++)
                {
                    crows.Add(ADSD.ltree[BaeysTREE.rules[st].treeid].nodes[BaeysTREE.rules[st].gbranch[r + 1]].rows.Count);
                }
                //В скобочках какие-то значения хуй знает о чем это вобоще
                BaeysTREE.rules[st].probability = BAYES.Gain(crows, ((4) - (5));
                crows.Clear();
            }
            ADSD.dthr.Remove(thrid);
            if (ADSD.dthr.Count == 0 && !tflag)
            {
                tflag = true;
                ftr.FillBaeysRes();
            }
         }

        void crtr()//create thread rules
        {
            ftr.CreatRules(pointer);
            for (int q = 0; q < ADSD.lthr.Count; q++)
            {
                if (ADSD.lthr[q].thrid == this.thrid)
                    ADSD.lthr.RemoveAt(q);
            }
            this.thr.Join();
        }

        void thrmcrgtree()
        {
            for (int p = 0; p < ADSD.ltree.Count; p++)
            {
                ADSD.ltree[p].crtgtree();
                ftr.spl(p);
                if (ftr.printrulescb.Checked)
                {
                    THREAD th = new THREAD(ftr, p);
                }
            }
            if (adaboost)
            {
                for (int t = 0; t < ADSD.ltree.Count; t++)
                {
                    ftr.mgboost(t);
                }
              ftr.FillAdaBoostRes();
            }
            while (ADSD.lthr.Count > 0)
                Thread.Sleep(10);
            ftr.UpdateReportText();
            if (ftr.usednfbox.Checked)
            {
                BaeysTREE btree = new BaeysTREE(ftr);
            }
            ftr.UpdateReportText(true);
            this.thr.Join();
        }

        void thrmcrtree()
        {
            while (this.tryes < 1)
            {
                if (ADSD.qnodes.Count == 0)
                {
                    ++this.tryes;
                    Thread.Sleep(100);
                    thrmcrtree();
                }
                else
                {
                    if (ADSD.qnodes.Count != 0)
                    {
                        NODE crnode = ADSD.qnodes.Dequeue();
                        ftr.srncol(ref crnode, crnode.tra);
                    }
                }
            }
            for (int i = 0; i < ADSD.lthr.Count; i++)
            {
                if (ADSD.lthr[i].alg == this.alg)
                    ADSD.lthr.RemoveAt(i);
            }
            if (ADSD.lthr.Count > 0) 
              this.thr.Join();
            if (ADSD.lthr.Count == 0 && ADSD.firstthread)
            {
                ADSD.firstthread = false;
                this.ftr.mcrgtr();
                this.thr.Join();
            }
        }   
    }

    public class BaeysTREE
    {
        public static List<GTREE> rules = new List<GTREE>();
        public static List<string> dnf = new List<string>();
        ADSD ftr;

        public BaeysTREE(ADSD ft)
        {
            ftr = ft;
            mdnf();
        }

        public void mdnf()//make dnf
        {
            bool canadd = false;
            for (int tr = 0; tr < ADSD.ltree.Count; tr++)
            {
                for (int br = 0; br < ADSD.ltree[tr].gtree.Count; br++)
                {
                    if (ADSD.ltree[tr].gtree[br].cl1)
                    {
                        if (rules.Count == 0)
                        {
                            rules.Add(ADSD.ltree[tr].gtree[br]);
                            rules[0].treeid = tr;
                        }
                        else
                        {
                            for (int i = 0; i < rules.Count; i++)
                            {
                                canadd = compare(rules[i], ADSD.ltree[tr].gtree[br]);
                            }
                            if (canadd)
                            {
                                rules.Add(ADSD.ltree[tr].gtree[br]);
                                rules[rules.Count - 1].treeid = tr;
                            }
                        }
                    }
                }
            }
            optimaze();
            crrep();//create report
        }

        private void crrep()
        {
            for (int el = 0; el < rules.Count; el++)
            {
                for (int subel = 0; subel < rules[el].gbranch.Count - 1; subel++)
                {
                    dnf.Add(ftr.gcn(ADSD.ltree[rules[el].treeid].nodes[rules[el].gbranch[subel]].column));
                    if (ADSD.ltree[rules[el].treeid].nodes[rules[el].gbranch[subel + 1]].value.Contains(' '))
                        dnf.Add(FUNC.parsCL(ADSD.ltree[rules[el].treeid].nodes[rules[el].gbranch[subel + 1]].value));
                    else
                        dnf.Add(config.CONVTOCHAR(ADSD.ltree[rules[el].treeid].nodes[rules[el].gbranch[subel + 1]].value));
                }
                dnf.Add("%^%");
            }
        }

        private void optimaze()
        {
            for (int rol = 0; rol < rules.Count; rol++)
            {
                config.SYSMES = ADSD.ltree[rules[rol].treeid].algname.ToString() + " : ";
                for (int subrol = 0; subrol < rules[rol].gbranch.Count() - 1; subrol++)
                {
                    config.SYSMES += ADSD.ltree[rules[rol].treeid].nodes[rules[rol].gbranch[subrol]].column.ToString() + " " + ADSD.ltree[rules[rol].treeid].nodes[rules[rol].gbranch[subrol + 1]].value + " -> ";
                }
            }
        }

        private bool compare(GTREE augt/*allready use*/, GTREE gt)//return true - if two branch not equels
        {
            if (augt.gbranch.Count == gt.gbranch.Count)
            {
                for (int el = 0; el < augt.gbranch.Count; el++)
                {
                    if (ADSD.ltree[augt.treeid].nodes[augt.gbranch[el]].column == ADSD.ltree[gt.treeid].nodes[gt.gbranch[el]].column)
                        continue;
                    else
                        return true;
                }
                return false;
            }
            else
                return true;
        }
    }

}
