using IsLabApp.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<NoteService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World");

app.MapGet("/health", () => new { status = "ok", timestamp = DateTime.Now });

app.MapGet("/version", () => new { name = "IsLabApp", version = "0.1.0" });

app.MapGet("/api/notes", (NoteService service) => service.GetAll());

app.MapGet("/api/notes/{id}", (int id, NoteService service) =>
{
    var note = service.GetById(id);
    return note is not null ? Results.Ok(note) : Results.NotFound();
});

app.MapPost("/api/notes", (CreateNoteRequest request, NoteService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest("Title is required");

    var note = service.Create(request.Title, request.Text);
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.MapDelete("/api/notes/{id}", (int id, NoteService service) =>
{
    return service.Delete(id) ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/db/ping", async (IConfiguration config) =>
{
    var connectionString = config.GetConnectionString("Mssql");
    try
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return Results.Json(new { status = "ok", message = "Database connected" });
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "error", message = ex.Message });
    }
});

app.Run();

public record CreateNoteRequest(string Title, string Text);