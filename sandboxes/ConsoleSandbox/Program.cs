using ConsoleSandbox;
using ConsoleSandbox.Commands;

using LasseVK.Extensions.Hosting;
using LasseVK.Extensions.Hosting.ConsoleApplications;

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.RelocateConfigurationFiles<Program>();
builder.AddCommandLineCommand<ListCommand>();

IHost host = builder.Build();
await host.RunAsync();