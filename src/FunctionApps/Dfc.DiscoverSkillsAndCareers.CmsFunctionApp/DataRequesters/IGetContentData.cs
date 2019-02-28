using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public interface IGetContentData<T> where T : class
    {
        Task<T> GetData(string url);
    }
}
