namespace IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework.Mappings;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderModificationStateMap : SagaClassMap<OrderModificationState>
{
    protected override void Configure(EntityTypeBuilder<OrderModificationState> entity, ModelBuilder model)
    {
        model.HasDefaultSchema("orders");
    }
}
