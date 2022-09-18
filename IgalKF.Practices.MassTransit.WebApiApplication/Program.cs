using Igalkf.Practices.Masstransit.DatabaseModel;
using Igalkf.Practices.MassTransit.EntityModel;
using IgalKF.Practices.MassTransit.MessagingModel;
using IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Verte.Samples.MassTransit.Poc.Database.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OrderStateDbContext>(x =>
{
    var connectionString = builder.Configuration.GetConnectionString("MasstransitPersistence");

    x.UseSqlServer(connectionString, options =>
    {
        options.EnableRetryOnFailure(5);
        options.MinBatchSize(1);
        options.MigrationsAssembly(typeof(Initialize).Assembly.FullName);
        options.MigrationsHistoryTable("Migrations");
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrderStateDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);

        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.AutoStart = true;
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/send", async (
    [FromServices] ISendEndpointProvider sendEndpointProvider,
    [FromServices] IPublishEndpoint publishEndpoint,
    [FromServices] OrderStateDbContext orderStateDbContext) =>
{
    for (int i = 0; i < 100; i++)
    {
        int domainNumber = new Random().Next(1, 2);
        ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new($"exchange:domain{domainNumber}-order-generation"));
        var order = new OrderEntity
        {
            Domain = $"domain{domainNumber}",
            OrderDate = DateTime.Now,
            OrderId = Guid.NewGuid(),
        };

        await orderStateDbContext
            .Set<OrderEntity>()
            .AddAsync(order);

        await orderStateDbContext.SaveChangesAsync();

        var ordersubmitted = new OrderSubmitted
        {
            Domain = $"domain{domainNumber}",
            OrderDate = DateTime.Now,
            OrderId = Guid.NewGuid(),
        };

        await sendEndpoint.Send(ordersubmitted);
    }

    return "Published";
})
.WithName("SendMessage");

await app.RunAsync();