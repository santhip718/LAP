using LAP.Domain.Entity;

namespace LAP.Application.Interface.IRepository;

/// <summary>
/// Defines data access operations for forum messages.
/// </summary>
public interface IForumRepository : IBaseRepository<ForumMessage>
{
}
