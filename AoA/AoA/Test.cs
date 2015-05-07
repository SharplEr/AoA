using System;
using System.IO;
using Metaheuristics;

namespace AoA
{
    /// <summary>
    /// Информация о выполненном тесте
    /// </summary>
    [Serializable]
    public class Test
    {
        public CVlog log;
        public String[] parameterNames;
        public object[] parameter;
        public String name;
        public bool ready = false;

        public Test(Type type, Parameter[] ps, CVlog l, object[] os)
        {
            if (ps.Length != os.Length) throw new ArithmeticException();

            log = l;
            name = type.ToString();
            parameterNames = new String[ps.Length];
            for (int i = 0; i < ps.Length; i++)
                parameterNames[i] = ps[i].name;
            
            parameter = os;
        }

        public bool Save(StreamWriter writer)
        {
            try
            {
                writer.WriteLine("   Тестирование алгоритма: {0}", name);
                writer.WriteLine();
                writer.WriteLine("Найденные параметры:");
                for (int i = 0; i < parameter.Length; i++)
                    writer.WriteLine("{0}: {1}.", parameterNames[i], parameter[i]);
                writer.WriteLine();
                if (log.Save(writer)) return true;
                else
                {
                    writer.Dispose();
                    return false;
                }
            }

            catch
            {
                writer.Dispose();
                return false;
            }
        }
    }
}
