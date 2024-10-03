using WebApiRequestLogs.DbContexts;

namespace WebApiRequestLogs.Services
{
    public interface IRequestResponseLogger
    {
        //void Log(IRequestResponseLogModelCreator logCreator);
        void Log(string message);
    }

    public class RequestResponseLogger : IRequestResponseLogger
    {
        //private readonly ILogger<RequestResponseLogger> _logger;
        private readonly AppDbContext _appDbContext;
        public RequestResponseLogger(
            //ILogger<RequestResponseLogger> logger,
            AppDbContext appDbContext)
        {
            //_logger = logger;
            _appDbContext = appDbContext;
        }
        //public void Log(IRequestResponseLogModelCreator logCreator)
        //{ 
        //    _logger.LogCritical(logCreator.LogString());
        //}
        public void Log(string message)
        {

        }
    }
}
