namespace SecondGeneration;

public interface IFilterContext<out TSource>
{
    IQueryable<TSource> Filter();
    Task<IEnumerable<object>> Inspect();
}