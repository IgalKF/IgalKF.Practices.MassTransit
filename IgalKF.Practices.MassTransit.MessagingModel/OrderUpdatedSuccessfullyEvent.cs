namespace IgalKF.Practices.MassTransit.MessagingModel;

public class OrderUpdatedSuccessfullyEvent
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string Domain { get; set; }
}