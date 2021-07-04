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
        /// <param name="relativeFilePaths"></param>
        /// <returns></returns>
        string GetAbsolutePath(params string[] relativeFilePaths);
    }
}