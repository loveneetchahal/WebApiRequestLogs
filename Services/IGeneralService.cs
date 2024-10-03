using Microsoft.EntityFrameworkCore;
using WebApiRequestLogs.DbContexts;
using WebApiRequestLogs.Models;

namespace WebApiRequestLogs.Services
{
    public interface IGeneralService
    {
        public bool AddLogs(RequestLogs requestLogs);
        public bool UpdateLogs(RequestLogs requestLogs);
    }
    public class GeneralService : IGeneralService
    {
        private readonly AppDbContext _dbContext;
        public GeneralService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddLogs(RequestLogs requestLogs)
        {
            _dbContext.RequestLogs.Add(requestLogs);
            _dbContext.SaveChanges();
            return true;
        }
        public bool UpdateLogs(RequestLogs requestLogs)
        {
            _dbContext.RequestLogs.Update(requestLogs);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
