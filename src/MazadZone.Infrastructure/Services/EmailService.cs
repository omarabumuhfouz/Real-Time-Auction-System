using MazadZone.Application.Common.Interfaces;

namespace MazadZone.Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(EmailRequest request, CancellationToken ct = default)
    {
        // Adding visual spacing and borders makes it easy to spot in a busy console
        Console.WriteLine("\n==========================================================");
        Console.WriteLine(" 📧 MOCK EMAIL DISPATCHED (Testing Mode)");
        Console.WriteLine("==========================================================");
        Console.WriteLine($" TO:      {request.To}");
        Console.WriteLine($" SUBJECT: {request.Subject}");
        Console.WriteLine($" FORMAT:  {(request.IsHtml ? "HTML" : "Plain Text")}");
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine(" BODY:");
        Console.WriteLine(request.Body);
        Console.WriteLine("==========================================================\n");

        // Return a completed task since we are no longer throwing an exception
        return Task.CompletedTask;
    }
}