using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerificationProvider_Rika.Functions
{
    public class GenerateVerificationCode
    {
        private readonly ILogger<GenerateVerificationCode> _logger;

        public GenerateVerificationCode(ILogger<GenerateVerificationCode> logger)
        {
            _logger = logger;
        }

        [Function(nameof(GenerateVerificationCode))]
        public async Task Run(
            [ServiceBusTrigger("verification_request", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
