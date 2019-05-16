namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
	public class SaveProgressViewModel : BaseViewModel
	{
		public string SessionId { get; set; }
		public string Code { get; set; }
		public string SessionDate { get; set; }
		public string Status { get; set; }

		public string SentTo { get; set; }
        public string BackLink { get; set; }
    }
}
