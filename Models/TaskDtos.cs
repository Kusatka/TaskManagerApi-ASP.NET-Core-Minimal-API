namespace TaskManagerApi.Models;

/// <summary>
/// Тело запроса для создания задачи (POST /api/tasks).
/// Id генерируется сервером, IsCompleted по умолчанию = false.
/// </summary>
public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Тело запроса для обновления задачи (PUT /api/tasks/{id}).
/// </summary>
public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
