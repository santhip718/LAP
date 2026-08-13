using LAP.Domain.Entity;

namespace LAP.Application.Interface.IRepository;

/// <summary>
/// Extends the generic repository with enrollment-specific data access methods.
/// </summary>
public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
}
