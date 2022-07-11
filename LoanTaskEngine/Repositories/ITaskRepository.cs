using LoanTaskEngine.Entities;
using LoanTaskEngine.Tasks;

namespace LoanTaskEngine.Repositories;

public interface ITaskRepository
{
    void AddTask(EntityTask task);
    EntityTask? GetTask(string taskId);
    IEnumerable<EntityTask> GetTasks(EntityType? entityType = null);
}