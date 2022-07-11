using System.Collections.Concurrent;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Tasks;

namespace LoanTaskEngine.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<string, EntityTask> _tasks = new(StringComparer.OrdinalIgnoreCase);
    private int _lastId = -1;

    public TaskRepository()
    {
    }

    public TaskRepository(IEnumerable<EntityTask>? tasks)
    {
        if (tasks is not null)
        {
            foreach (var task in tasks)
            {
                AddTask(task);
            }
        }
    }

    public EntityTask? GetTask(string taskId)
    {
        _tasks.TryGetValue(taskId, out var task);
        return task;
    }

    public IEnumerable<EntityTask> GetTasks(EntityType? entityType = null)
    {
        foreach (var pair in _tasks)
        {
            var task = pair.Value;
            if (entityType is null || entityType == task.Entity)
            {
                yield return task;
            }
        }
    }

    public void AddTask(EntityTask task)
    {
        if (!string.IsNullOrEmpty(task.Id))
        {
            throw new ArgumentException("Cannot add an existing task", nameof(task));
        }
        task.Id = Interlocked.Increment(ref _lastId).ToString();
        _tasks.TryAdd(task.Id, task);
    }
}