namespace UrlShortner.Common;

public interface IQueryHandler<TInput, TResult>
{
    Task<TResult> Handle(TInput input);
}

public interface IQueryHandler<T>
{
    Task<T> Handle(T input);
}

