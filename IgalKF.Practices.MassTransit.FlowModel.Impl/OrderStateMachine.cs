namespace IgalKF.Practices.MassTransit.FlowModel.Impl;

using global::MassTransit;
using IgalKF.Practices.MassTransit.MessagingModel;

public class OrderStateMachine :
    MassTransitStateMachine<OrderGenerationState>
{
    public State Submitted { get; private set; }
    public State Accepted { get; private set; }
    public State Completed { get; private set; }
    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderAccepted> OrderAccepted { get; private set; }

    public OrderStateMachine()
    {
        Event(() => this.OrderSubmitted, x => x.CorrelateById(state => state.Message.OrderId));
        Event(() => this.OrderAccepted, x => x.CorrelateById(state => state.Message.OrderId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
                .ThenAsync(async context => await ProcessOrder(context))
                .TransitionTo(Submitted));

        During(Submitted,
            When(OrderAccepted)
                .ThenAsync(async context => await ProcessOrder(context))
                .TransitionTo(Accepted));

        During(Accepted,
            When(OrderAccepted)
                .Then(context => { Console.WriteLine($"Completed, Wow! {context.Saga.CorrelationId}"); })
                .TransitionTo(Completed)
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
}
