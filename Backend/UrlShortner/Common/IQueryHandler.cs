namespace UrlShortner.Common;

public interface IQueryHandler<TInput, TResult> where TInput : class, IQuery
{
    Task<TResult> Handle(TInput input);
}

public interface IQueryHandler<TResult>
{
    Task<TResult> Handle();
}
