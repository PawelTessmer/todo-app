using System.Text.Json;

using TodoApp;

List<TodoTask> tasks = new List<TodoTask>();

if (File.Exists("tasks.json"))
{
    string json = File.ReadAllText("tasks.json");
    var loadedTasks = JsonSerializer.Deserialize<List<TodoTask>>(json);

    if (loadedTasks != null)
    {
        tasks = loadedTasks;
    }
}

static void WriteColor(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}

string filter = "all";

while (true)
{
    Console.Clear();
    string filterLabel = filter switch
    {
        "completed" => "Completed",
        "pending" => "Pending",
        _ => "All"
    };

    WriteColor($"Filter: {filterLabel}", ConsoleColor.DarkGray);

    ShowTasks(tasks, filter);

    WriteColor("\n=== TODO APP ===", ConsoleColor.Cyan);
    Console.WriteLine("\n1. Add task");
    Console.WriteLine("2. Mark as done");
    Console.WriteLine("3. Delete task");
    Console.WriteLine("4. Show completed");
    Console.WriteLine("5. Show pending");
    Console.WriteLine("6. Show all");
    Console.WriteLine("7. Exit");

    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            AddTask(tasks);
            break;

        case "2":
            MarkDone(tasks,filter);
            break;

        case "3":
            DeleteTask(tasks, filter);
            break;

        case "4":
            filter = "completed";
            break;

        case "5":
            filter = "pending";
            break;

        case "6":
            filter = "all";
            break;

        case "7":
            return;

        default:
            WriteColor("Invalid input", ConsoleColor.Red);
            Pause();
            break;
    }
    
}

static void Pause()
{
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

static void AddTask(List<TodoTask> tasks)
{
    Console.WriteLine("Enter task title: ");
    string title = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(title))
    {
        WriteColor("Task cannot be empty!", ConsoleColor.Red);
        Pause();
        return;
    }

    if(tasks.Any(t => string.Equals(t.Title.Trim(), title.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        WriteColor("Task with this name already exists!", ConsoleColor.Yellow);
        Pause();
        return;
    }

    tasks.Add(new TodoTask(title));
    SaveTasks(tasks);
    Console.WriteLine("Task added!");

    Pause();
}

static void ShowTasks(List<TodoTask> tasks, string filter)
{

    var filteredTasks = GetFilteredTasks(tasks, filter);

    if (filteredTasks.Count == 0)
    {
        WriteColor("No tasks to display.", ConsoleColor.Yellow);
        return;
    }

    Console.WriteLine("\n--- TASKS ---");

    for (int i = 0; i < filteredTasks.Count; i++)
    {
        var task = filteredTasks[i];

        Console.ForegroundColor = task.IsDone ? ConsoleColor.Green : ConsoleColor.Red;

        string status = task.IsDone ? "[X]" : "[ ]";
        Console.WriteLine($"{i}. {status} {task.Title} ({task.CreatedAt:yyyy-MM-dd HH:mm})");

        Console.ResetColor();
    }
}

static void MarkDone(List<TodoTask> tasks, string filter)
{
    if (tasks.Count == 0)
    {
        WriteColor("No tasks available.", ConsoleColor.Yellow);
        Pause();
        return;
    }

    Console.WriteLine("Choose task index: ");

    if (!int.TryParse(Console.ReadLine(), out int index))
    {
        WriteColor("Invalid input!", ConsoleColor.Red);
        Pause();
        return;
    }

    var filteredTasks = GetFilteredTasks(tasks, filter);

    if (index < 0 || index >= filteredTasks.Count)
    {

        WriteColor("Invalid index!", ConsoleColor.Red);
        Pause();
        return;
    }

    var selectedTask = filteredTasks[index];
    if (selectedTask.IsDone)
    {
        WriteColor("Task already completed!", ConsoleColor.Yellow);
        Pause();
        return;
    }

    selectedTask.IsDone = true;
    SaveTasks(tasks);

    WriteColor("Task marked as done!", ConsoleColor.Green);
    Pause();
}

static void DeleteTask(List<TodoTask> tasks, string filter)
{
    if (tasks.Count == 0)
    {
        WriteColor("No tasks available.", ConsoleColor.Yellow);
        Pause();
        return;
    }

    Console.WriteLine("Choose task index: ");
    if (!int.TryParse(Console.ReadLine(), out int index))
    {
        WriteColor("Invalid input!", ConsoleColor.Red);
        Pause();
        return;
    }

    var filteredTasks = GetFilteredTasks(tasks, filter);

    if (index < 0 || index >= filteredTasks.Count)
    {
        WriteColor("Invalid index!", ConsoleColor.Red);
        Pause();
        return;
    }
    var taskToDelete = filteredTasks[index];

    tasks.Remove(taskToDelete);
    SaveTasks(tasks);

    WriteColor("Task deleted!", ConsoleColor.Green);
    Pause();
}

static List<TodoTask> GetFilteredTasks(List<TodoTask> tasks, string filter)
{
    if (filter == "completed")
        return tasks.Where(t => t.IsDone).ToList();
    else if (filter == "pending")
        return tasks.Where(t => !t.IsDone).ToList();

    return tasks;
}

static void SaveTasks(List<TodoTask> tasks)
{
    string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
    {
        WriteIndented = true
    });
    File.WriteAllText("tasks.json", json);
}