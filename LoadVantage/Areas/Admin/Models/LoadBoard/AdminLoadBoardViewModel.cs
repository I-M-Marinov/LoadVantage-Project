using LoadVantage.Areas.Admin.Models.Load;
using LoadVantage.Areas.Admin.Models.Profile;
using LoadVantage.Core.Models.Load;

namespace LoadVantage.Areas.Admin.Models.LoadBoard
{
    public class AdminLoadBoardViewModel
	{
		public Guid UserId { get; set; }
		public AdminProfileViewModel Profile { get; set; } = null!;
		public IEnumerable<AdminLoadViewModel> CreatedLoads { get; set; }
		public IEnumerable<AdminLoadViewModel> PostedLoads { get; set; }
		public IEnumerable<AdminLoadViewModel> BookedLoads { get; set; }
		public IEnumerable<AdminLoadViewModel> DeliveredLoads { get; set; }
		public IEnumerable<AdminLoadViewModel> CancelledLoads { get; set; }
		
	}
}
