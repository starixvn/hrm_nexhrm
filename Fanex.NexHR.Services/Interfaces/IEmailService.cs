namespace Fanex.NexHR.Services.Interfaces;

public interface IEmailService
{
    void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body);
}