using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace GistSync.Core.Controllers
{
    [ApiController]
    public class SyncTasksController : ControllerBase
    {
        private readonly GistSyncDbContext _dbContext;

        public SyncTasksController(GistSyncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("/sync-tasks")]
        public JsonResult GetAll()
        {
            return new JsonResult(_dbContext.SyncTasks.Select(t => new {
                t.GistId,
                t.SyncStrategyType,
                FileName = t.GistFileName,
                LocalFile = t.MappedLocalFilePath,
                AccessToken = t.GitHubPersonalAccessToken
            }).ToArray());
        }
    }
}