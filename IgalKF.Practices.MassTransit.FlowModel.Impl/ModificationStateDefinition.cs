namespace IgalKF.Practices.MassTransit.FlowModel.Impl;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;
using IgalKF.Practices.MassTransit.PersistenceModel.EntityFramework;

public class ModificationStateDefinition : SagaDefinition<OrderModificationState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OrderModificationState> sagaConfigurator)
    {
        IPartitioner partition = endpointConfigurator.CreatePartitioner(200);

        sagaConfigurator.Message<UpdateOrderEvent>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
        sagaConfigurator.Message<OrderUpdatedSuccessfullyEvent>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
    }
}