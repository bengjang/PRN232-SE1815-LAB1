using PRN232.LMS.Services.BusinessModels;

namespace PRN232.LMS.Services.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardBusinessModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
