namespace IgalKF.Practices.MassTransit.FlowModel.Impl;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;

public class RegistrationStateDefinition : SagaDefinition<OrderGenerationState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<OrderGenerationState> sagaConfigurator)
    {
        IPartitioner partition = endpointConfigurator.CreatePartitioner(200);

        sagaConfigurator.Message<OrderSubmitted>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
        sagaConfigurator.Message<OrderAccepted>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
    }
}
