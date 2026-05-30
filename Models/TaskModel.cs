namespace TaskManagerApi.Models;

/// <summary>
/// Модель данных задачи. Соответствует структуре записи в Data/tasks.json.
/// </summary>
public class TaskModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
