using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PRN232.LMS.Course.Service.Messaging;

public interface IEnrollmentEventPublisher
{
    Task PublishEnrollmentCompletedAsync(EnrollmentCompletedEvent message, CancellationToken cancellationToken = default);
}

public record EnrollmentCompletedEvent(
    int EnrollmentId,
    int StudentId,
    int CourseId,
    DateTime EnrollDate,
    string Status);

public class EnrollmentEventPublisher : IEnrollmentEventPublisher
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<EnrollmentEventPublisher> _logger;

    public EnrollmentEventPublisher(IOptions<RabbitMqOptions> options, ILogger<EnrollmentEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishEnrollmentCompletedAsync(EnrollmentCompletedEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: _options.EnrollmentCompletedRoutingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "Published enrollment event {EnrollmentId} to RabbitMQ exchange {ExchangeName}.",
                message.EnrollmentId,
                _options.ExchangeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish enrollment event {EnrollmentId} to RabbitMQ.", message.EnrollmentId);
        }

        return Task.CompletedTask;
    }
}
