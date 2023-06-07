namespace SecondGeneration;

public interface IFilter<TSource>
{
    IFilterContext<TSource> Build(IQueryable<TSource> queryable, IEnumerable<NamedFilter> filterModels);
}