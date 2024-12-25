using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.StateManager;

namespace DST.Bot.Features.Common;

public class UserHelper
{
    private readonly AppDbContext _dbContext;

    public UserHelper(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public  Task UpdateUserState(User user, DialogStateId dialogState)
    {
        user.DialogStateId = dialogState;
        return Task.FromResult(_dbContext.SaveChangesAsync());
    }
}