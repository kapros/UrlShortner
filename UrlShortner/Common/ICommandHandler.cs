namespace UrlShortner.Common;

public interface ICommandHandler<TInput, TResult>
{
    Task<TResult> Handle(TInput input);
}

public interface ICommandHandler<T>
{
    Task Handle(T input);
}

