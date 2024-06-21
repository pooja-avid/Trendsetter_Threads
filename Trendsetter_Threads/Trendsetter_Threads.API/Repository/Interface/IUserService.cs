using Trendsetter_Threads.API.Data.Models;

namespace Trendsetter_Threads.API.Repository.Interface;
public interface IUserService
{
    public Task<OperationResult> RegisterUser(UserModel model);
    public Task<OperationResult<UserModel>> Login(LoginModel model);
}
