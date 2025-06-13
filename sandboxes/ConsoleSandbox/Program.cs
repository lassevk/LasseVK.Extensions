using LasseVK.Extensions.Hosting;
using LasseVK.Extensions.Hosting.ConsoleApplications;

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.RelocateConfigurationFiles<Program>();
builder.AddCommandLineApplication<CommandLineApplication>();

IHost host = builder.Build();
await host.RunAsync();