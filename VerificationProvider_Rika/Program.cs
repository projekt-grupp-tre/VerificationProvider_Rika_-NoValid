using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VerificationProvider_Rika.Data.Contexts;
using VerificationProvider_Rika.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("VerificationDatabase")));
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IVerificationCleanerService, VerificationCleanerService>();
        services.AddScoped<IValidateVerificationCodeService, ValidateVerificationCodeService>();

    })
    .Build();

host.Run();