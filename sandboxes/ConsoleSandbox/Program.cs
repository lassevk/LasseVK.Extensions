using System.Reflection;

using LasseVK.Extensions.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.RelocateConfigurationFiles<Program>();

IHost host = builder.Build();
string? value = host.Services.GetRequiredService<IConfiguration>()["Key"];
Console.WriteLine($"value: '{value ?? "<null>"}'");