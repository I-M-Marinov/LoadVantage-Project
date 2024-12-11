using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.LoadBoard
{
	public class LoadBoardViewModel
	{
		public Guid UserId { get; set; }
		public ProfileViewModel Profile { get; set; } = null!;
		public IEnumerable<LoadViewModel> CreatedLoads { get; set; } = null!;
		public IEnumerable<LoadViewModel> PostedLoads { get; set; } = null!;
		public IEnumerable<LoadViewModel> BookedLoads { get; set; } = null!;
		public IEnumerable<DeliveredLoadViewModel> DeliveredLoads { get; set; } = null!;
	}
		
}
