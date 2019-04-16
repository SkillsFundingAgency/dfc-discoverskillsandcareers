namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class IndexViewModel
    {
        public string SessionId { get; set; }
        public bool HasReloadError { get; set; }
        public string Title { get; set; }
        public string Headline { get; set; }
        public string TextBlock1 { get; set; }
        public string TextBlock2 { get; set; }
        public string TextHighlight { get; set; }
        public string TextBlock3 { get; set; }
        public string ResumeTitle { get; set; }
        public string ResumeFieldTitle { get; set; }
        public string ResumeButtonText { get; set; }
        public string ResumeErrorMessage { get; set; }
        public string MissingCodeErrorMessage { get; set; }
    }
}
