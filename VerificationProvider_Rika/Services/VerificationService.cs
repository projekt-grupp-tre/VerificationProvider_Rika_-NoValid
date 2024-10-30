

using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider_Rika.Data.Contexts;
using VerificationProvider_Rika.Models;

namespace VerificationProvider_Rika.Services;

public class VerificationService(ILogger<VerificationService> logger, IServiceProvider serviceProvider) : IVerificationService
{
    private readonly ILogger<VerificationService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var verificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(message.Body.ToString());
            if (verificationRequest != null && !string.IsNullOrEmpty(verificationRequest.Email))
                return verificationRequest;

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : UnpackVerificationRequest.Run :: {ex.Message}", ex);

        }
        return null!;
    }

    public string GenerateCode()
    {
        try
        {
            var rdn = new Random();
            var code = rdn.Next(100000, 999999).ToString();
            return code.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : GenerateCode.GenerateCode() :: {ex.Message}", ex);

        }
        return null!;

    }


    public async Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DataContext>();

            var existingRequest = await context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
            if (existingRequest != null)
            {
                existingRequest.Code = code;
                existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                context.Entry(existingRequest).State = EntityState.Modified;
            }
            else
            {
                context.VerificationRequests.Add(new Data.Entities.VerificationRequestEntity() { Email = verificationRequest.Email, Code = code });
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : SaveVerificationRequest.SaveVerificationRequest() :: {ex.Message}", ex);
            return false;
        }
    }


    public EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code)
    {
        try
        {
            if (!string.IsNullOrEmpty(verificationRequest.Email) && !string.IsNullOrEmpty(code))
            {
                var emailRequest = new EmailRequest
                {
                    To = verificationRequest.Email,
                    Subject = $"Verification Code{code}",
                    Body = $"<h1>Your verification code is {code}</h1>",
                    PlainText = $"Your verification code is {code}"
                };
                return emailRequest;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.GenerateEmailRequest() :: {ex.Message}", ex);

        }
        return null!;
    }


    public string GenerateServiceBusEmailRequest(EmailRequest emailRequest)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
            {
                return payload;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : GenerateServiceBusEmailRequest.GenerateServiceBusEmailRequest() :: {ex.Message}", ex);

        }
        return null!;
    }
}
