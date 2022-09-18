namespace IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework;

using global::MassTransit;
using global::MassTransit.EntityFrameworkCoreIntegration;
using Igalkf.Practices.MassTransit.EntityModel;
using IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework.Mappings;
using Microsoft.EntityFrameworkCore;

public class OrderStateDbContext
    : SagaDbContext
{
    public OrderStateDbContext(DbContextOptions<OrderStateDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.Entity<OrderEntity>()
            .HasKey(o => o.OrderId);

        modelBuilder.Entity<OrderEntity>()
            .HasIndex(x => new
            {
                x.MemberId,
                x.EventId,
            }).IsUnique();
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new OrderGenerationStateMap();
            yield return new OrderModificationStateMap();
        }
    }
}