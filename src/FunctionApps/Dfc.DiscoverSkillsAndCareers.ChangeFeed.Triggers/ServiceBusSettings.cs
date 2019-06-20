namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Triggers
{
    public class ServiceBusSettings
    {
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
