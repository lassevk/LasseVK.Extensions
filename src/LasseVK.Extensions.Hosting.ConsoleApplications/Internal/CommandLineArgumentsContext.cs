using LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal class CommandLineArgumentsContext
{
    private readonly Dictionary<string, ICommandLineProperty> _optionHandlers;
    private readonly List<ICommandLineProperty> _positionalHandlers;

    private ICommandLineProperty? _currentHandler;
    private string? _currentOption;
    private bool _parseOptions = true;

    public CommandLineArgumentsContext(Dictionary<string, ICommandLineProperty> optionHandlers, List<ICommandLineProperty> positionalHandlers)
    {
        _optionHandlers = optionHandlers ?? throw new ArgumentNullException(nameof(optionHandlers));
        _positionalHandlers = positionalHandlers ?? throw new ArgumentNullException(nameof(positionalHandlers));
    }

    public List<string> Errors { get; } = [];

    public void Process(string argument)
    {
        if (_parseOptions && (argument.StartsWith('-') || argument.StartsWith('/')))
        {
            ProcessOption(argument);
        }
        else
        {
            ProcessValue(argument);
        }
    }

    private void ProcessOption(string option)
    {
        EndPreviousOption();

        if (option == "--")
        {
            return;
        }

        string trimmedOption = option.TrimStart('-', '/');
        string optionValue = "";
        int parameterIndex = trimmedOption.IndexOfAny([':', '=']);
        if (parameterIndex >= 0)
        {
            optionValue = trimmedOption[(parameterIndex + 1) ..];
            trimmedOption = trimmedOption[..parameterIndex];
        }

        if (!_optionHandlers.TryGetValue(trimmedOption, out ICommandLineProperty? handler))
        {
            Errors.Add($"No command line option {option} found");
            return;
        }

        _currentOption = option;
        _currentHandler = handler;

        if (optionValue != "")
        {
            ProcessValueForOption(optionValue);
        }
    }

    private void EndPreviousOption()
    {
        if (_currentHandler == null)
        {
            return;
        }

        switch (_currentHandler.Finish())
        {
            case ArgumentHandlerFinishResponse.Finished:
                _currentHandler = null;
                _currentOption = null;
                break;

            case ArgumentHandlerFinishResponse.MissingValue:
                Errors.Add($"Missing value for option {_currentOption}");
                break;
        }
    }

    private void ProcessValue(string value)
    {
        if (_currentHandler != null)
        {
            ProcessValueForOption(value);
            return;
        }

        if (_positionalHandlers.Count > 0)
        {
            ProcessValueWithFirstPositional(value);
            return;
        }

        Errors.Add($"Don't know what to do with command line argument {value}");
    }

    private void ProcessValueWithFirstPositional(string value)
    {
        switch (_positionalHandlers[0].Accept(value))
        {

            case ArgumentHandlerAcceptResponse.ContinueAccepting:
                break;

            case ArgumentHandlerAcceptResponse.Finished:
                _positionalHandlers.RemoveAt(0);
                break;

            case ArgumentHandlerAcceptResponse.StopParsing:
                _positionalHandlers.RemoveAt(0);
                _parseOptions = false;
                break;

            case ArgumentHandlerAcceptResponse.InvalidValue:
                Errors.Add($"Invalid value for {_positionalHandlers[0].Name}: {value}");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessValueForOption(string value)
    {
        switch (_currentHandler!.Accept(value))
        {
            case ArgumentHandlerAcceptResponse.ContinueAccepting:
                break;

            case ArgumentHandlerAcceptResponse.Finished:
                _currentHandler = null;
                _currentOption = null;
                break;

            case ArgumentHandlerAcceptResponse.InvalidValue:
                Errors.Add($"Invalid value for option {_currentOption}: {value}");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Finish()
    {
        EndPreviousOption();

        foreach (ICommandLineProperty handler in _positionalHandlers)
        {
            if (handler.Finish() == ArgumentHandlerFinishResponse.MissingValue)
            {
                Errors.Add($"Missing value for argument {handler.Name}");
            }
        }
    }
}