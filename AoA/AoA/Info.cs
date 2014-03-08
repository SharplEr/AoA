using System.Collections.Concurrent;

namespace AoA
{
    /// <summary>
    /// Информация об экспериментах над одним примером
    /// </summary>
    public class Info
    {
        public BlockingCollection<double> errorControl = new BlockingCollection<double>();
        public BlockingCollection<double> errorLearn = new BlockingCollection<double>();
        
        public double avgErrorControl;
        public double avgErrorLearn;
        public double errorOfErrorControl;
        public double errorOfErrorLearn;
        public int nClass;

        public Info() {}
    }
}
