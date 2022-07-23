namespace GistSync.Core.Strategies.Contracts
{
    public interface ISyncStrategy
    {
        void Setup(int syncTaskId);
        void Destroy();
    }
}