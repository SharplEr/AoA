namespace Metaheuristics
{
    public interface IJump<T> where T : IQuality<T>
    {
        bool Jump(T x, T y);
    }
}
