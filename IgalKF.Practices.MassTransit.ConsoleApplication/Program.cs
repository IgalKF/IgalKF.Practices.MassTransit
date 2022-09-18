namespace IgalKF.Practices.MassTransit.ConsoleApplication
{
    using global::MassTransit;
    using Igalkf.Practices.Masstransit.DatabaseModel;
    using IgalKF.Practices.MassTransit.FlowModel.Impl;
    using IgalKF.Practices.MassTransit.MessagingModel;
    using IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Program
    {
        static bool? isRunningInContainer;

        static bool IsRunningInContainer =>
            isRunningInContainer ??= bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inContainer) && inContainer;

        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                OrderStateDbContext orderStateDbContext =
                    scope.ServiceProvider.GetRequiredService<OrderStateDbContext>();

                await orderStateDbContext.Database.MigrateAsync();

                var orderId = NewId.Next().ToString();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionString = hostContext.Configuration.GetConnectionString("MasstransitPersistence");

                    services.AddDbContext<OrderStateDbContext>(builder =>
                        builder.UseSqlServer(connectionString, m =>
                        {
                            m.MigrationsAssembly(typeof(MigrationAssembly).Assembly.FullName);
                            m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                        }));


                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        x.AddSagaStateMachinesFromNamespaceContaining<OrderStateMachine>();

                        x.AddEntityFrameworkOutbox<OrderStateDbContext>(o =>
                        {
                            o.DisableInboxCleanupService();
                            o.QueryDelay = TimeSpan.FromSeconds(1);
                            o.UseSqlServer();
                            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                            o.UseBusOutbox(bo => bo.DisableDeliveryService());
                        });

                        x.SetEntityFrameworkSagaRepositoryProvider(r =>
                        {
                            r.ExistingDbContext<OrderStateDbContext>();
                            r.UseSqlServer();
                        });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            foreach (string domain in new string[] { "domain1", "domain2" })
                            {
                                foreach (Type type in typeof(OrderGenerationState).Assembly.GetTypes())
                                {
                                    if (type.GetInterface("SagaStateMachineInstance") is null) continue;

                                    string queueSuffix = type.Name.Replace("State", string.Empty).PascalToKebabCase();
                                    string queueName = $"{domain}-{queueSuffix}";

                                    cfg.ReceiveEndpoint(queueName, c =>
                                    {
                                        c.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                                        c.UseMessageRetry(r => r.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));

                                        c.UseEntityFrameworkOutbox<OrderStateDbContext>(context);

                                        c.ConfigureSaga(context, type);
                                    });
                                }

                                cfg.ConfigureEndpoints(context);
                            }
                        });
                    });
                });
    }

    public static class StringExtensions
    {
        public static string PascalToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }
    }
}