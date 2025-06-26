using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core.Services
{
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<char, ICommand> _commands = new();

        public CommandRegistry()
        {
            RegisterDefaultCommands();
        }

        private void RegisterDefaultCommands()
        {
            Register(new TurnLeftCommand());
            Register(new TurnRightCommand());
            Register(new MoveForwardCommand());
        }

        public void Register(ICommand command)
        {
            _commands[command.Symbol] = command;
        }

        public ICommand GetCommand(char symbol)
        {
            if (_commands.TryGetValue(symbol, out var command))
                return command;

            throw new InvalidOperationException($"Unknown command: '{symbol}'");
        }
    }
}
