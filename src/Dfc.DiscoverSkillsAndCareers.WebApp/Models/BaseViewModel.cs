namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class BaseViewModel
    {
        public string ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string Layout { get; set; }
    };
}