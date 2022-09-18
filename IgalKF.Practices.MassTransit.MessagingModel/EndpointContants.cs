namespace IgalKF.Practices.MassTransit.MessagingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndpointUris
{
    public static Uri OrderGeneration(string domain) => new($"exchange:{domain}-order-generation");
    public static Uri OrderModification(string domain) => new($"exchange:{domain}-order-modification");
}
