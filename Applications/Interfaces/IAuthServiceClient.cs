using Api.Contracts.Auth;

namespace Applications.Interfaces
{
    public interface IAuthServiceClient
    {
        Task<string> ProvisionUserAsync(CreateUserRequest request);
    }
}
