namespace UrlShortner.Common;

public interface ICommandHandler<TInput, TResult>
{
    Task<TResult> Handle(TInput input);
}

public interface ICommandHandler<T>
{
    Task<T> Handle(T input);
}

