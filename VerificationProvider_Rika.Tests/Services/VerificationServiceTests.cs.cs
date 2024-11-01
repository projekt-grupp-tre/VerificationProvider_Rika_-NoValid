using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace VerificationProvider_Rika.Tests.Services
{
    
    public interface IEmailProvider
    {
        Task SendEmail(string userEmail, string message);
    }

   
    public class VerificationService
    {
        private readonly IEmailProvider _emailProvider;
        private string _currentVerificationCode;

        public VerificationService(IEmailProvider emailProvider)
        {
            _emailProvider = emailProvider;
        }

      
        public async Task<string> GenerateAndSendVerificationCode(string userEmail)
        {
            _currentVerificationCode = new Random().Next(100000, 999999).ToString("D6");
            await _emailProvider.SendEmail(userEmail, $"Your verification code is {_currentVerificationCode}");
            return _currentVerificationCode;
        }

        
        public bool ValidateVerificationCode(string code)
        {
            return code == _currentVerificationCode;
        }
    }

    public class VerificationServiceTests
    {
        private readonly Mock<IEmailProvider> _mockEmailProvider;
        private readonly VerificationService _verificationService;

        public VerificationServiceTests()
        {
            
            _mockEmailProvider = new Mock<IEmailProvider>();
            _verificationService = new VerificationService(_mockEmailProvider.Object);
        }

        [Fact]
        public async Task GenerateAndSendVerificationCode_ShouldGenerateCodeAndSendEmail()
        {
            // Arrange
            string userEmail = "testuser@example.com";
            _mockEmailProvider
                .Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            string generatedCode = await _verificationService.GenerateAndSendVerificationCode(userEmail);

            // Assert
            Assert.NotNull(generatedCode); 
            Assert.Equal(6, generatedCode.Length);
            _mockEmailProvider.Verify(e => e.SendEmail(userEmail, It.Is<string>(msg => msg.Contains(generatedCode))), Times.Once);
        }

        [Fact]
        public void ValidateVerificationCode_ShouldReturnTrue_WhenCodeIsCorrect()
        {
            // Arrange
            string generatedCode = _verificationService.GenerateAndSendVerificationCode("testuser@example.com").Result;

            // Act
            bool isValid = _verificationService.ValidateVerificationCode(generatedCode);

            // Assert
            Assert.True(isValid); // Should be valid for the correct code
        }

        [Fact]
        public void ValidateVerificationCode_ShouldReturnFalse_WhenCodeIsIncorrect()
        {
            // Arrange
            _verificationService.GenerateAndSendVerificationCode("testuser@example.com").Wait();
            string incorrectCode = "000000";

            // Act
            bool isValid = _verificationService.ValidateVerificationCode(incorrectCode);

            // Assert
            Assert.False(isValid); // Should be invalid for the incorrect code
        }
    }
}
