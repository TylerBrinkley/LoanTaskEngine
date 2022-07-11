using LoanTaskEngine.Actions;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Tasks;
using Newtonsoft.Json;

var tasksFile = "tasks.json";
if (args.Length > 0)
{
    tasksFile = args[0];
}

var actionsFile = "actions.json";
if (args.Length > 1)
{
    actionsFile = args[1];
}

var tasks = JsonConvert.DeserializeObject<List<EntityTask>>(File.ReadAllText(tasksFile))!;
var actions = JsonConvert.DeserializeObject<List<EntityAction>>(File.ReadAllText(actionsFile))!;

var taskRepo = new TaskRepository(tasks);
var loanRepo = new LoanRepository(taskRepo);

Entity? loan = null;
var entities = new HashSet<Entity>();
foreach (var action in actions)
{
    var entity = action.Execute(loanRepo);
    entities.Add(entity);
    loan ??= entity;
    Console.WriteLine(JsonConvert.SerializeObject(action));
    Console.WriteLine(JsonConvert.SerializeObject(loan, Formatting.Indented));
    foreach (var e in entities)
    {
        foreach (var taskStatusPair in e.TaskStatuses)
        {
            var task = taskRepo.GetTask(taskStatusPair.Key);
            Console.WriteLine($"Task Name: '{task!.Name}' Entity: '{e.Id}' Status: {taskStatusPair.Value}");
        }
    }
    Console.WriteLine();
}