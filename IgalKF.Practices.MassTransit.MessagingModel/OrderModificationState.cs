namespace IgalKF.Practices.MassTransit.MessagingModel;

using global::MassTransit;

public class OrderModificationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public DateTime OrderDate { get; set; }

    public string Domain { get; set; }

    public string Error { get; set; }

}