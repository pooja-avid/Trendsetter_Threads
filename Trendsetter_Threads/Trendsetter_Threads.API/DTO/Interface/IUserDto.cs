using Trendsetter_Threads.API.Data.Models;

namespace Trendsetter_Threads.API.DTO.Interface;

public interface IUserDto
{
    public Task<OperationResult> RegisterUser(UserModel model);
    public Task<OperationResult<UserModel>> Login(LoginModel model);
}
