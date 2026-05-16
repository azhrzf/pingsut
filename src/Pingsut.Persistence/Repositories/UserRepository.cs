using Microsoft.EntityFrameworkCore;
using Pingsut.App.Contracts.Persistence;
using Pingsut.Domain.Entities;
using Pingsut.Persistence.DbContexts;

namespace Pingsut.Persistence.Repositories;

public class UserRepository(PostgreDbContext postgreDbContext)
    : BaseRepository<User>(postgreDbContext), IUserRepository;