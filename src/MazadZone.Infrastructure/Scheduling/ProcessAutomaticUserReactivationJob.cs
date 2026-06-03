using System.ComponentModel;
using Hangfire;
using MazadZone.Application.Users.BackgroundJobs;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MazadZone.Infrastructure.Scheduling;

public class ProcessAutomaticUserReactivationJob : IAutomaticUserReactivationJob
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessAutomaticUserReactivationJob> _logger;

    public ProcessAutomaticUserReactivationJob(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<ProcessAutomaticUserReactivationJob> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    // The {0} matches the first parameter passed to the method (userId)
    [DisplayName("Automatically Reactivating Suspended User ID: {0}")]
    [AutomaticRetry(Attempts = 3)] // Retries 3 times if the DB is locked or down
    public async Task ExecuteAsync(Guid userId, CancellationToken cancellationToken)
{
    Console.WriteLine("\n==========================================================");
    Console.WriteLine(" ⏳ HANGFIRE JOB: STARTING USER REACTIVATION");
    Console.WriteLine($" TARGET USER ID: {userId}");
    Console.WriteLine("==========================================================");

    var stronglyTypedId = UserId.From(userId);
        
    // 1. Fetch
    var user = await _userRepository.GetByIdAsync(stronglyTypedId, cancellationToken);
    
    if (user is null)
    {
        Console.WriteLine($" ❌ JOB FAILED: User {userId} not found in database.");
        return; 
    }

    // 2. Mutate
    Console.WriteLine($" 🔄 Current Status: {user.Status}");
    var result = user.Activate();
    
    if (result.IsFailure)
    {
        Console.WriteLine($" ⚠️ JOB SKIPPED: Activation failed. Error: {result.TopError.Message}");
        return; 
    }

    // 3. Save
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    
    Console.WriteLine(" ✅ JOB SUCCESS: User status updated to ACTIVE.");
    Console.WriteLine("==========================================================\n");
}
}