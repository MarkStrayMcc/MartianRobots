namespace Martian.Robots.Core.Abstractions
{
    public interface ICommandRegistry
    {
        ICommand GetCommand(char symbol);
        void Register(ICommand command);
    }
}