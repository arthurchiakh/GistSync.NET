namespace GistSync.Core.Services.Contracts
{
    public interface IAppDataService
    {
        /// <summary>
        /// Create application directory
        /// </summary>
        void CreateAppDirectory();
        /// <summary>
        /// Delete application directory
        /// </summary>
        void DeleteAppDirectory();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        string GetRelativePath(params string[] paths);
    }
}