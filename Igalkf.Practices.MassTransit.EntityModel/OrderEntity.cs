namespace Igalkf.Practices.MassTransit.EntityModel;

/// <summary>
/// Order entity.
/// </summary>
public class OrderEntity
{
    /// <summary>
    /// Gets or sets order's identifier.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets order's date.
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets requet's domain.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Gets or sets member identifier.
    /// </summary>
    public string MemberId { get; set; } = null!;

    /// <summary>
    /// Gets or sets event identifier.
    /// </summary>
    public string EventId { get; set; } = null!;
}