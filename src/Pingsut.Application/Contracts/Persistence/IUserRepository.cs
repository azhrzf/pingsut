using Pingsut.Domain.Entities;

namespace Pingsut.Application.Contracts.Persistence;

public interface IUserRepository : IAsyncRepository<User>;