namespace UrlShortner.Common;

public interface IQueryHandler<TInput, TResult> where TInput : class, IQuery
{
    Task<TResult> Handle(TInput input);
}
