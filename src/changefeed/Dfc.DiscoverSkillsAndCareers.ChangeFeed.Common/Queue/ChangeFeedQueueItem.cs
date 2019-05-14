namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Queue
{
    public class ChangeFeedQueueItem
    {
        public string BlobName { get; set; }
        public string Type { get; set; }
    }
}
