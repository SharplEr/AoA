using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Metaheuristics;

namespace AoA
{
    public static class AlgorithmFactory
    {
        public static Func<object[], Algorithm> GetFactory(Type T)
        {
            if (!typeof(Algorithm).IsAssignableFrom(T)) throw new ArgumentException("Не верный тип, должен наследовать Algorithm");

            if (T.IsAbstract) throw new ArgumentException("Класс должен быть не абстрактным");

            return (x) => (Algorithm)Activator.CreateInstance(T, x);
        }

        public static Func<object[], Algorithm>[] GetFactory(params Type[] Ts)
        {
            if (Ts == null) throw new ArgumentNullException("GetFactory: Пустой массив типов");

            Func<object[], Algorithm>[] ans = new Func<object[],Algorithm>[Ts.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = GetFactory(Ts[i]);

            return ans;
        }

        public static Type[] LoadFromDLL(string file)
        {
            Assembly a = Assembly.LoadFrom(file);
            List<Type> ans = new List<Type>();

            foreach (Type t in a.GetExportedTypes())
            {
                if (!t.IsAbstract && typeof(Algorithm).IsAssignableFrom(t))
                    ans.Add(t);
            }

            return ans.ToArray();
        }

        public static Type[] LoadFromDLL(string file, string name)
        {
            Assembly a = Assembly.LoadFrom(file);
            List<Type> ans = new List<Type>();

            foreach (Type t in a.GetExportedTypes())
            {
                if (!t.IsAbstract && typeof(Algorithm).IsAssignableFrom(t) && t.ToString()==name)
                    ans.Add(t);
            }

            return ans.ToArray();
        }
        /// <summary>
        /// Возвращает в первой позиции тип алгоритма, во второй тип границ исследования
        ///  или null если исследовать не требуется.
        /// </summary>
        /// <param name="name">Имя файла метаданных</param>
        /// <returns></returns>
        public static Tuple<Type, Type> LoadAlgorithmInfo(string name)
        {
            using (var reader = new StreamReader(name))
            {    
                //Считываем имя файла для алгоритма
                string nameDll = reader.ReadLine();
                reader.ReadLine();
                //Считываем имя класса
                string nameType = reader.ReadLine();
                reader.ReadLine();
                var t = LoadFromDLL(nameDll, nameType);
                if (t.Length != 1)
                    throw new ArgumentException("LoadAlgorithmInfo: неверное число алгоритмов: "+t.Length);
                Type tAlg = t[0];

                if (reader.EndOfStream)

                //Считываем имя файла для границ параметров
                nameDll = reader.ReadLine();
                if (reader.EndOfStream)
                    return new Tuple<Type, Type>(tAlg,null);
                reader.ReadLine();
                //Считываем имя класса для границ параметров
                nameType = reader.ReadLine();

                t = LoadFromDLL(nameDll, nameType);

                if (t.Length != 1)
                    throw new ArgumentException("LoadAlgorithmInfo: неверное число сетеров: " + t.Length);

                Type tParam = t[0];
                return new Tuple<Type, Type>(tAlg,tParam);
            }
        }
    }
}
