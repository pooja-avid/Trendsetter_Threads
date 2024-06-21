using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.DTO.Interface;
using Trendsetter_Threads.API.Repository.Interface;

namespace Trendsetter_Threads.API.DTO.Service;

public class UserDto : IUserDto
{
    private readonly IUserService _user;
 
    public UserDto(IUserService userService)
    {
        this._user = userService;
    }

    public Task<OperationResult<UserModel>> Login(LoginModel model)
    {
        return _user.Login(model);
    }

    public Task<OperationResult> RegisterUser(UserModel model)
    {
        return _user.RegisterUser(model);
    }
}
