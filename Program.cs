using TaskManagerApi.Models;
using TaskManagerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Сервисы ---

// Swagger / OpenAPI для тестирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Хранилище задач (работа с JSON-файлом). Singleton — один экземпляр на всё приложение.
builder.Services.AddSingleton(new TaskRepository(builder.Environment.ContentRootPath));

var app = builder.Build();

// --- Middleware ---

// Swagger UI доступен по адресу /swagger
app.UseSwagger();
app.UseSwaggerUI();

// Перенаправление с корня на Swagger UI для удобства
app.MapGet("/", () => Results.Redirect("/swagger"));

// =====================================================================
//  Endpoints (CRUD)
//  Endpoint — только обработчик HTTP-запроса. Вся работа с данными
//  делегируется в TaskRepository.
// =====================================================================

// 1. Получить список всех задач
app.MapGet("/api/tasks", (TaskRepository repo) =>
{
    return Results.Ok(repo.GetAll());
})
.WithName("GetAllTasks")
.WithTags("Tasks");

// Доп. задание: получить только выполненные задачи.
// ВАЖНО: маршрут должен быть объявлен ДО "/api/tasks/{id}",
// иначе "completed" будет воспринят как {id}.
app.MapGet("/api/tasks/completed", (TaskRepository repo) =>
{
    return Results.Ok(repo.GetCompleted());
})
.WithName("GetCompletedTasks")
.WithTags("Tasks");

// 2. Получить задачу по Id
app.MapGet("/api/tasks/{id:int}", (int id, TaskRepository repo) =>
{
    var task = repo.GetById(id);
    return task is not null
        ? Results.Ok(task)
        : Results.NotFound(new { message = $"Задача с id={id} не найдена." });
})
.WithName("GetTaskById")
.WithTags("Tasks");

// 3. Создать новую задачу
app.MapPost("/api/tasks", (TaskCreateDto dto, TaskRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { message = "Поле 'title' обязательно." });

    var created = repo.Create(dto);
    return Results.Created($"/api/tasks/{created.Id}", created);
})
.WithName("CreateTask")
.WithTags("Tasks");

// 4. Обновить задачу
app.MapPut("/api/tasks/{id:int}", (int id, TaskUpdateDto dto, TaskRepository repo) =>
{
    var updated = repo.Update(id, dto);
    return updated is not null
        ? Results.Ok(updated)
        : Results.NotFound(new { message = $"Задача с id={id} не найдена." });
})
.WithName("UpdateTask")
.WithTags("Tasks");

// 5. Удалить задачу
app.MapDelete("/api/tasks/{id:int}", (int id, TaskRepository repo) =>
{
    var deleted = repo.Delete(id);
    return deleted
        ? Results.NoContent()
        : Results.NotFound(new { message = $"Задача с id={id} не найдена." });
})
.WithName("DeleteTask")
.WithTags("Tasks");

app.Run();
