namespace UrlShortner.Common;

public interface ICommandHandler<TInput, TResult> where TInput : class, ICommand
{
    Task<TResult> Handle(TInput input);
}

public interface ICommandHandler<T> where T : class, ICommand
{
    Task Handle(T input);
}
