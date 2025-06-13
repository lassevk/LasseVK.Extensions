namespace LasseVK.Extensions.Hosting.ConsoleApplications.Handlers;

internal record struct PositionalArgumentHandler(int Position, IArgumentHandler Handler);