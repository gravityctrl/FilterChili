using System.Numerics;

namespace SecondGeneration;

public interface IEntityConfigurator<TSource>
{
    IElementFilterConfigurator Property(Expression<Func<TSource, string>> selector);

    INumberFilterConfigurator Property<TValue>(Expression<Func<TSource, TValue>> selector)
        where TValue : INumber<TValue>, IMinMaxValue<TValue>;
    
    IMatchTypeConfigurator<IElementFilterConfigurator> Property(Expression<Func<TSource, IEnumerable<string>>> selector);
    
    IMatchTypeConfigurator<INumberFilterConfigurator> Property<TValue>(Expression<Func<TSource, IEnumerable<TValue>>> selector) 
        where TValue : INumber<TValue>, IMinMaxValue<TValue>;
}