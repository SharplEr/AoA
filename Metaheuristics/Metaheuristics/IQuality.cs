namespace Metaheuristics
{
    //Такое сложное объявление на самом деле имеет глубокий смысл и запрещает неправильное использование
    //Хотя к сожалению не все его варианты
    public interface IQuality<T> where T : IQuality<T>
    {
        //Из this вычесть o
        double CompareTo(T o);
    }
}