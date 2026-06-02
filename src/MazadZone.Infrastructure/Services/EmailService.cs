using MazadZone.Application.Common.Interfaces;
using MazadZone.Infrastructure.Common;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp; // Replaces System.Net.Mail
using MailKit.Security;

namespace MazadZone.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly GmailOptions _options;

    public EmailService(IOptions<GmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(EmailRequest request, CancellationToken ct = default)
    {
        // 1. Build the MimeKit Message
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("MazadZone", _options.Email));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = request.Subject;

        // 2. Wrap the body in your HTML template
        string finalBody = request.IsHtml ? GetStandardHtmlTemplate(request.Body) : request.Body;

        var builder = new BodyBuilder();
        if (request.IsHtml)
            builder.HtmlBody = finalBody;
        else
            builder.TextBody = finalBody;
        
        email.Body = builder.ToMessageBody();

        // 3. Send using MailKit's SmtpClient
        using var smtp = new SmtpClient();
        
        // This line is the magic that makes Port 465 work!
        await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect, ct);
        
        await smtp.AuthenticateAsync(_options.Email, _options.Password, ct);
        
        await smtp.SendAsync(email, ct);
        
        await smtp.DisconnectAsync(true, ct);
    }

    private string GetStandardHtmlTemplate(string messageBody)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        </head>
        <body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f4f7f6; color: #333333;'>
            <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color: #f4f7f6; padding: 20px;'>
                <tr>
                    <td align='center'>
                        <table width='100%' max-width='600px' cellpadding='0' cellspacing='0' border='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);'>
                            
                            <tr>
                                <td style='background-color: #0b1120; padding: 25px 20px; text-align: center; border-bottom: 3px solid #fc6e06;'>
                                    <h1 style='margin: 0; font-size: 28px; font-weight: bold; letter-spacing: 0.5px;'>
                                        <span style='color: #ffffff;'>Mazad</span><span style='color: #fc6e06;'>Zone</span>
                                    </h1>
                                </td>
                            </tr>
                            
                            <tr>
                                <td style='padding: 30px; line-height: 1.6; font-size: 16px; color: #1a1a1a;'>
                                    {messageBody}
                                </td>
                            </tr>
                            
                            <tr>
                                <td style='background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 13px; color: #666666; border-top: 1px solid #eeeeee;'>
                                    <p style='margin: 0;'>&copy; {DateTime.Now.Year} MazadZone. All rights reserved.</p>
                                    <p style='margin: 5px 0 0 0;'>This is an automated system message. Please do not reply.</p>
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }
}