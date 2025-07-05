using System.Threading.Tasks;

namespace MySampleApi.Services;

// Interface for the Service Bus sender service
public interface IServiceBusSenderService
{
    Task SendMessageAsync(string messageContent);
}