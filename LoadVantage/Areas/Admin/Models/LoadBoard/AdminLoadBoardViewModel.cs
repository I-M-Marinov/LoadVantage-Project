using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.Profile;

namespace LoadVantage.Areas.Admin.Models.LoadBoard
{
    public class AdminLoadBoardViewModel
	{
		public Guid UserId { get; set; }
		public AdminProfileViewModel Profile { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> CreatedLoads { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> PostedLoads { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> BookedLoads { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> DeliveredLoads { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> CancelledLoads { get; set; } = null!;
		
	}
}
