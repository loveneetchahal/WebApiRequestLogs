using Newtonsoft.Json;
using WebApiRequestLogs.Models;

namespace WebApiRequestLogs.Services
{
    public interface IRequestResponseLogModelCreator
    {
        RequestResponseLogModel LogModel { get; }
        string LogString();
    }

    public class RequestResponseLogModelCreator : IRequestResponseLogModelCreator
    {
        public RequestResponseLogModel LogModel { get; private set; }

        public RequestResponseLogModelCreator()
        {
            LogModel = new RequestResponseLogModel();
        }

        public string LogString()
        {
            var jsonString = JsonConvert.SerializeObject(LogModel);
            return jsonString;
        }
    }
}
