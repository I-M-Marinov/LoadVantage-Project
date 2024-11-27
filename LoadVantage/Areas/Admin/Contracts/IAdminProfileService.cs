using LoadVantage.Areas.Admin.Models;

namespace LoadVantage.Areas.Admin.Contracts
{
    public interface IAdminProfileService
    {
        Task<AdminProfileViewModel?> GetAdminInformation(Guid adminId);

    }
}
