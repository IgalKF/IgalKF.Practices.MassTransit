namespace IgalKF.Practices.MassTransit.FlowModel.Impl;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;

public class OrderModificationStateMachine :
    MassTransitStateMachine<OrderModificationState>
{
    public State Accepted { get; private set; }
    public State Completed { get; private set; }
    public Event<UpdateOrderEvent> UpdateOrderEvent { get; private set; }
    public Event<OrderUpdatedSuccessfullyEvent> OrderUpdatedSuccessfullyEvent { get; private set; }

    public OrderModificationStateMachine()
    {
        Event(() => this.UpdateOrderEvent, x => x.CorrelateById(state => state.Message.OrderId));
        Event(() => this.OrderUpdatedSuccessfullyEvent, x => x.CorrelateById(state => state.Message.OrderId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(UpdateOrderEvent)
                .ThenAsync(async context => await ProcessOrder(context))
                .TransitionTo(Completed));

        During(Completed,
            When(OrderUpdatedSuccessfullyEvent)
                .ThenAsync(async context => await ProcessOrder(context))
                .Finalize());

        this.SetCompletedWhenFinalized();
    }

    public static async Task ProcessOrder(BehaviorContext<OrderGenerationState, OrderSubmitted> context)
    {
        context.Saga.Domain = context.Message.Domain;
        context.Saga.OrderDate = DateTime.UtcNow;
        Console.WriteLine($"{context.Saga.CurrentState}! {context.Saga.CorrelationId} for domain: {context.Message.Domain}");
        await context.Send(EndpointUris.OrderGeneration(context.Message.Domain), new OrderAccepted(context.Saga.CorrelationId));
    }

    public static async Task ProcessOrder(BehaviorContext<OrderGenerationState, OrderAccepted> context)
    {
        try
        {
            if (new Random().Next(1, 6) != 1) throw new Exception("Random exception");
            context.Saga.OrderDate = DateTime.UtcNow;
            Console.WriteLine($"{context.Saga.CurrentState}! {context.Saga.CorrelationId}  for domain: {context.Saga.Domain}");
            await context.Send(EndpointUris.OrderGeneration(context.Saga.Domain), new OrderAccepted(context.Saga.CorrelationId));
        }
        catch (Exception ex)
        {
            context.Saga.Error = ex.Message;
        }
    }

    public static async Task ProcessOrder(BehaviorContext<OrderModificationState, UpdateOrderEvent> context)
    {
        try
        {
            context.Saga.Domain = context.Message.Domain;
            context.Saga.OrderDate = DateTime.UtcNow;
            if (new Random().Next(1, 6) != 1) throw new Exception("Random exception");
            Console.WriteLine($"{context.Saga.CurrentState}! {context.Saga.CorrelationId}  for domain: {context.Saga.Domain}");
            await context.Send(EndpointUris.OrderModification(context.Saga.Domain), new OrderUpdatedSuccessfullyEvent { OrderId = context.Saga.CorrelationId });
        }
        catch (Exception ex)
        {
            context.Saga.Error = ex.Message;
        }
    }

    public static async Task ProcessOrder(BehaviorContext<OrderModificationState, OrderUpdatedSuccessfullyEvent> context)
    {
        Console.WriteLine($"{context.Saga.CurrentState}! {context.Saga.CorrelationId}  for domain: {context.Saga.Domain} ------- Lunched a SignalR Message!");
        await Task.CompletedTask;
    }
}
