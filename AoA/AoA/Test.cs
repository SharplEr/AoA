using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoA
{
    /// <summary>
    /// Информация о выполненном тесте
    /// </summary>
    [Serializable]
    public class Test
    {
        public CVlog log;
        public object[] parameter;
        public bool ready = false;
    }
}
