using System.ComponentModel;
using System.Numerics;
using System.Reflection;

using LasseVK.Extensions.Hosting.ConsoleApplications.Attributes;
using LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

namespace LasseVK.Extensions.Hosting.ConsoleApplications.Internal;

internal static class CommandLineArgumentsReflector
{
    private static readonly Dictionary<Type, Func<PropertyInfo, object, string, string?, ICommandLineProperty?>> _argumentHandlers = [];

    static CommandLineArgumentsReflector()
    {
        _argumentHandlers.Add(typeof(List<string>), StringListCommandLineProperty.Factory);
        _argumentHandlers.Add(typeof(bool), BooleanCommandLineProperty.Factory);
        _argumentHandlers.Add(typeof(bool?), BooleanCommandLineProperty.Factory);

        _argumentHandlers.Add(typeof(string), StringCommandLineProperty.Factory);
        _argumentHandlers.Add(typeof(Guid), GuidCommandLineProperty.Factory);

        addNumberHandler<byte>();
        addNumberHandler<sbyte>();
        addNumberHandler<ushort>();
        addNumberHandler<short>();
        addNumberHandler<uint>();
        addNumberHandler<int>();
        addNumberHandler<ulong>();
        addNumberHandler<long>();
        addNumberHandler<UInt128>();
        addNumberHandler<Int128>();
        addNumberHandler<Half>();
        addNumberHandler<float>();
        addNumberHandler<double>();
        addNumberHandler<decimal>();

        addNumberHandler<BigInteger>();

        // DateTime?
        // DateOnly?
        // TimeOnly?
        // TimeSpan?
        // Char?

        void addNumberHandler<T>()
            where T : struct, INumber<T>
        {
            _argumentHandlers.Add(typeof(T), NumericCommandLineProperty<T>.Factory);
            _argumentHandlers.Add(typeof(T?), NumericCommandLineProperty<T>.Factory);
        }
    }

    private static IEnumerable<PropertyInfo> GetEligibleProperties(object instance)
    {
        foreach (PropertyInfo property in instance.GetType().GetProperties())
        {
            if (property is not { CanRead: true })
            {
                continue;
            }

            if (property.GetIndexParameters() is not [])
            {
                continue;
            }

            yield return property;
        }
    }

    public static Dictionary<string, ICommandLineProperty> ReflectOptionProperties(object instance)
    {
        Dictionary<string, ICommandLineProperty> result = [];

        foreach (PropertyInfo property in GetEligibleProperties(instance))
        {
            if (!_argumentHandlers.TryGetValue(property.PropertyType, out Func<PropertyInfo, object, string, string?, ICommandLineProperty?>? handlerFactory))
            {
                continue;
            }

            var optionAttributes = property.GetCustomAttributes<OptionAttribute>().ToList();
            if (optionAttributes.Count == 0)
            {
                continue;
            }

            string name = property.GetCustomAttribute<ArgumentNameAttribute>()?.Name ?? property.Name;
            string? description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;

            ICommandLineProperty? handler = handlerFactory(property, instance, name, description);
            if (handler is null)
            {
                continue;
            }

            foreach (string option in optionAttributes.Select(x => x.Option))
            {
                result.Add(option, handler);
            }
        }

        return result;
    }

    public static List<PositionalArgumentHandler> ReflectPositionalProperties(object instance)
    {
        List<PositionalArgumentHandler> result = [];

        foreach (PropertyInfo property in GetEligibleProperties(instance))
        {
            if (!_argumentHandlers.TryGetValue(property.PropertyType, out Func<PropertyInfo, object, string, string?, ICommandLineProperty?>? handlerFactory))
            {
                continue;
            }

            PositionalArgumentAttribute? positionalAttribute = property.GetCustomAttribute<PositionalArgumentAttribute>();
            RestArgumentsAttribute? restAttribute = property.GetCustomAttribute<RestArgumentsAttribute>();
            if (positionalAttribute == null && restAttribute == null)
            {
                continue;
            }

            string name = property.GetCustomAttribute<ArgumentNameAttribute>()?.Name ?? property.Name;
            string? description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;

            ICommandLineProperty? handler = handlerFactory(property, instance, name, description);
            if (handler is null)
            {
                continue;
            }

            if (positionalAttribute != null)
            {
                result.Add(new PositionalArgumentHandler(positionalAttribute.Position, handler));
            }
            else if (restAttribute != null)
            {
                result.Add(new PositionalArgumentHandler(int.MaxValue, handler));
            }
        }

        return result;
    }
}