namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class StartViewModel
    {
        public string SessionId { get; set; }
        public string Title { get; set; } = "National Careers Service";
        public string Headline { get; set; } = "Job categories";
        public string Subheading { get; set; } = "Answer statements about what you enjoy, and discover job categories tailored to you";
        public string StartNowButtontext { get; set; } = "Start now";
        public string HighlightText { get; set; } = "This assessment will take 5 minutes";
        public string Content { get; set; } = "<p>On the following screens, we will ask you to agree or disagree with statements about yourself.</p><p>We’ll use your answers to suggest types of work you might be suited to.</p><p>Some of the statements are very similar. This is to help us make sure your results are more reliable. Try to answer quickly, even if you think you’ve seen a statement before.</p><p>You will get more accurate results if you answer honestly and respond to every statement. But you can use the option called ‘this doesn’t apply to me’ if you are finding it hard to respond to a particular statement.</p>";
    }
}
