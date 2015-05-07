namespace Metaheuristics
{
    public class TrueJumper<T>: IJump<T> where T : IQuality<T>
    {
        public TrueJumper()
        { }

        public bool Jump(T x, T y)
        {
            return true;
        }
    }
}
