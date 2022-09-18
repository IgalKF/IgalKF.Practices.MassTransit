namespace IgalKF.Practices.MassTransit.MessagingModel;

public class OrderAccepted
{
    public OrderAccepted(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
