using Azure.Messaging.ServiceBus; // Required for ServiceBusClient, ServiceBusSender, ServiceBusMessage
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using Microsoft.Extensions.Logging; // Required for ILogger
using System;
using System.Text.Json; // Required for JsonSerializer

namespace MySampleApi.Services;

// Implementation of the Service Bus sender service
public class ServiceBusSenderService : IServiceBusSenderService, IAsyncDisposable
{
    private readonly ServiceBusClient _serviceBusClient; // Client for interacting with Service Bus
    private readonly ServiceBusSender _serviceBusSender; // Sender for sending messages to a queue/topic
    private readonly ILogger<ServiceBusSenderService> _logger; // Logger for logging messages

    // Constructor for dependency injection
    public ServiceBusSenderService(IConfiguration configuration, ILogger<ServiceBusSenderService> logger)
    {
        _logger = logger;

        // Retrieve Service Bus connection string and queue name from configuration
        var connectionString = configuration["AzureServiceBus:ConnectionString"];
        var queueName = configuration["AzureServiceBus:QueueName"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Azure Service Bus ConnectionString is not configured.");
        }
        if (string.IsNullOrEmpty(queueName))
        {
            throw new ArgumentNullException(nameof(queueName), "Azure Service Bus QueueName is not configured.");
        }

        try
        {
            // Create a ServiceBusClient
            _serviceBusClient = new ServiceBusClient(connectionString);
            // Create a ServiceBusSender for the specified queue
            _serviceBusSender = _serviceBusClient.CreateSender(queueName);
            _logger.LogInformation($"ServiceBusSenderService initialized for queue: {queueName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize ServiceBusSenderService.");
            throw; // Re-throw to indicate a critical startup failure
        }
    }

    /// <summary>
    /// Sends a message to the configured Azure Service Bus queue.
    /// </summary>
    /// <param name="messageContent">The content of the message to send.</param>
    public async Task SendMessageAsync(string messageContent)
    {
        try
        {
            // Create a new Service Bus message
            // You can send raw strings, or serialize objects to JSON
            var message = new ServiceBusMessage(messageContent);

            // Send the message
            await _serviceBusSender.SendMessageAsync(message);
            _logger.LogInformation($"Message '{messageContent}' sent to Service Bus.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message '{messageContent}' to Service Bus.");
            throw; // Re-throw for upstream error handling
        }
    }

    /// <summary>
    /// Disposes the Service Bus sender and client when the service is no longer needed.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_serviceBusSender != null)
        {
            await _serviceBusSender.DisposeAsync();
            _logger.LogInformation("ServiceBusSender disposed.");
        }
        if (_serviceBusClient != null)
        {
            await _serviceBusClient.DisposeAsync();
            _logger.LogInformation("ServiceBusClient disposed.");
        }
        GC.SuppressFinalize(this); // Suppress finalization
    }
}