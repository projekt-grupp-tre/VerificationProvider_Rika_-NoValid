

using Microsoft.AspNetCore.Http;
using VerificationProvider_Rika.Models;

namespace VerificationProvider_Rika.Services;

public interface IValidateVerificationCodeService
{
    Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req);
    Task<bool> ValidateCodeAsync(ValidateRequest validateRequest);
}
