
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace LoginApp.Utils;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _email;

    public EmailSender(IOptions<EmailSettings> options)
    {
        _email = options.Value;
    }
    public void SendEmail(Mail request)
    {

        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_email.Email);
        email.To.Add(MailboxAddress.Parse(request.ToEmail));
        email.Subject = request.Subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = request.Body;
        email.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        client.Connect(_email.Host, _email.Port, SecureSocketOptions.StartTls);
        client.Authenticate(_email.Email, _email.Password);
        client.Send(email);
        client.Disconnect(true);
    }

}