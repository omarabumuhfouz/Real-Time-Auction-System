using Microsoft.Extensions.Logging;
using MazadZone.Application.Services;
using MazadZone.Domain.Repositories;

namespace Tests.Application.Features.Users;

public abstract class UserBaseTest<THandler> : GlobalTestBase
         where THandler : class
{
    protected readonly IUserQueries _userQueries; 

    protected readonly ILogger<THandler> Logger;
    protected readonly IPasswordService _passwordService;
    
    protected THandler Handler => AutoMocker.CreateInstance<THandler>();

    protected UserBaseTest()
    {
        _userQueries = AutoMocker.GetSubstituteFor<IUserQueries>();
        Logger = AutoMocker.GetSubstituteFor<ILogger<THandler>>();
        _passwordService = AutoMocker.GetSubstituteFor<IPasswordService>();
    }
}