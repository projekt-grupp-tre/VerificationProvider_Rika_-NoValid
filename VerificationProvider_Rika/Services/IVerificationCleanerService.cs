
namespace VerificationProvider_Rika.Services;

public interface IVerificationCleanerService
{
    Task RemoveExpiredRecordAsync();
}
