using System.Text.Json;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services;


public class TaskRepository
{
    private readonly string _filePath;


    private readonly object _lock = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // id, title, description, isCompleted
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public TaskRepository(string contentRootPath)
    {
        var dataDir = Path.Combine(contentRootPath, "Data");
        Directory.CreateDirectory(dataDir);

        _filePath = Path.Combine(dataDir, "tasks.json");

        // Если файла нет — создаём пустой массив, чтобы приложение стартовало корректно.
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    private List<TaskModel> ReadAll()
    {
        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<TaskModel>();

        return JsonSerializer.Deserialize<List<TaskModel>>(json, JsonOptions)
               ?? new List<TaskModel>();
    }

    private void WriteAll(List<TaskModel> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, JsonOptions);
        File.WriteAllText(_filePath, json);
    }


    public List<TaskModel> GetAll()
    {
        lock (_lock)
        {
            return ReadAll();
        }
    }

    public List<TaskModel> GetCompleted()
    {
        lock (_lock)
        {
            return ReadAll().Where(t => t.IsCompleted).ToList();
        }
    }

    public TaskModel? GetById(int id)
    {
        lock (_lock)
        {
            return ReadAll().FirstOrDefault(t => t.Id == id);
        }
    }

    public TaskModel Create(TaskCreateDto dto)
    {
        lock (_lock)
        {
            var tasks = ReadAll();

            //максимальный существующий + 1 (или 1, если пусто).
            var newId = tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1;

            var task = new TaskModel
            {
                Id = newId,
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false
            };

            tasks.Add(task);
            WriteAll(tasks);

            return task;
        }
    }

    /// <summary>Возвращает обновлённую задачу или null, если задача с таким id не найдена.</summary>
    public TaskModel? Update(int id, TaskUpdateDto dto)
    {
        lock (_lock)
        {
            var tasks = ReadAll();
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task is null)
                return null;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;

            WriteAll(tasks);
            return task;
        }
    }

    /// <summary>Возвращает true, если задача была удалена; false — если не найдена.</summary>
    public bool Delete(int id)
    {
        lock (_lock)
        {
            var tasks = ReadAll();
            var removed = tasks.RemoveAll(t => t.Id == id) > 0;

            if (removed)
                WriteAll(tasks);

            return removed;
        }
    }
}
