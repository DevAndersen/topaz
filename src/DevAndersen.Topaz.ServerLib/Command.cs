using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevAndersen.Topaz.ServerLib
{
    public class Command
    {
        public string RegexPattern { get; init; }

        public Func<Match, bool> CommandFunc { get; init; }

        // Todo: Implement priority.
        public int Priority { get; init; } = 0;

        /// <summary>
        /// Define a command's regex pattern, and the logic it will perform.
        /// Sends exit command to client after finishing action.
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="commandAction"></param>
        public Command(string regexPattern, Action<Match> commandAction)
        {
            RegexPattern = regexPattern;
            CommandFunc = (match) =>
            {
                commandAction.Invoke(match);
                return true;
            };
        }

        /// <summary>
        /// Define a command's regex pattern, and the logic it will perform.
        /// If <paramref name="commandFunc"/> returns true, sends exit command to client - necessary in the case of sending an exit- or error message.
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="commandFunc"></param>
        public Command(string regexPattern, Func<Match, bool> commandFunc)
        {
            RegexPattern = regexPattern;
            CommandFunc = commandFunc;
        }

        /// <summary>
        /// Define a command's regex pattern, and the logic it will perform.
        /// Sends exit command to client after finishing action.
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="priority"></param>
        /// <param name="commandAction"></param>
        public Command(string regexPattern, int priority, Action<Match> commandAction) : this(regexPattern, commandAction)
        {
            Priority = priority;
        }

        /// <summary>
        /// Define a command's regex pattern, and the logic it will perform.
        /// If <paramref name="commandFunc"/> returns true, sends exit command to client - necessary in the case of sending an exit- or error message.
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="priority"></param>
        /// <param name="commandFunc"></param>
        public Command(string regexPattern, int priority, Func<Match, bool> commandFunc) : this(regexPattern, commandFunc)
        {
            Priority = priority;
        }
    }
}
