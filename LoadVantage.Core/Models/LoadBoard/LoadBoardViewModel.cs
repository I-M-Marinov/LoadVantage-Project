using LoadVantage.Core.Models.Load;
using LoadVantage.Core.Models.Profile;

namespace LoadVantage.Core.Models.LoadBoard
{
	public class LoadBoardViewModel
	{
		public Guid UserId { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string CompanyName { get; set; } = null!;
		public string Position { get; set; } = null!;
		public ProfileViewModel Profile { get; set; } = null!;
		public IEnumerable<LoadViewModel> CreatedLoads { get; set; }
		public IEnumerable<LoadViewModel> PostedLoads { get; set; }
		public IEnumerable<LoadViewModel> BookedLoads { get; set; }
		public IEnumerable<LoadViewModel> BilledLoads { get; set; }
	}
		
}
