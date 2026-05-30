namespace TaskManagerApi.Models;


public class TaskCreateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
