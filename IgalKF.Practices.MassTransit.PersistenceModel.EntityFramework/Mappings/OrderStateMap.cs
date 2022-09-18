namespace IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework.Mappings;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderGenerationStateMap : SagaClassMap<OrderGenerationState>
{
    protected override void Configure(EntityTypeBuilder<OrderGenerationState> entity, ModelBuilder model)
    {
        model.HasDefaultSchema("orders");
        entity.Property(x => x.CurrentState)
            .HasMaxLength(64);
        entity.Property(x => x.OrderDate);
    }
}