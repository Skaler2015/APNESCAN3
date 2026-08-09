using ApneScan.ImportExport.Email;
using ApneScan.Sdk.Tests;
using NSubstitute;

namespace ApneScan.Lib.Tests.Automation;

internal class MockEmailProviderFactory : IEmailProviderFactory
{
    private Exception _assertException;

    public MockEmailProviderFactory(Action<EmailMessage> messageAsserts)
    {
        EmailProviderMock.SendEmail(Arg.Any<EmailMessage>(), Arg.Any<ProgressHandler>())
            .Returns(x =>
            {
                var message = (EmailMessage) x[0];
                try
                {
                    messageAsserts.Invoke(message);
                }
                catch (Exception ex)
                {
                    ex.PreserveStackTrace();
                    _assertException = ex;
                }
                return Task.FromResult(true);
            });
    }

    public IEmailProvider Create(EmailProviderType type) => EmailProviderMock;

    public IEmailProvider Default => EmailProviderMock;

    public IEmailProvider EmailProviderMock { get; } = Substitute.For<IEmailProvider>();

    public void CheckAsserts()
    {
        if (_assertException != null)
        {
            throw _assertException;
        }
    }

    public void VerifyExactlyOneMessageSent()
    {
        EmailProviderMock.Received().SendEmail(Arg.Any<EmailMessage>(), Arg.Any<ProgressHandler>());
        EmailProviderMock.ReceivedCallsCount(1);
    }
}