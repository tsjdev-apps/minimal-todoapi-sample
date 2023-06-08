using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Database;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoItems"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
{
    Version = "v1",
    Title = "Todo API",
    Description = "A sample ASP.NET Core Web API using the Minimal API approach for handling todo items",
    Contact = new OpenApiContact
    {
        Name = "tsjdev-apps.de",
        Url = new Uri("https://www.tsjdev-apps.de")
    },
    License = new OpenApiLicense
    {
        Name = "MIT License",
        Url = new Uri("https://opensource.org/licenses/MIT")
    }
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/alive", () => "Service is running...")
    .WithTags("General")
    .Produces(200, typeof(string));


app.MapGet("/todoitems", async (TodoDbContext db) =>
{
    return await db.TodoItems.ToListAsync();
})
    .WithTags("Todo")
    .Produces(200, typeof(List<TodoItem>));

app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);

    if (todoItem == null)
        return Results.NotFound();

    return Results.Ok(todoItem);
})
    .WithTags("Todo")
    .Produces(200, typeof(TodoItem))
    .Produces(404);

app.MapPost("/todoitems", async (TodoItem todoItem, TodoDbContext db) =>
{
    await db.TodoItems.AddAsync(todoItem);
    
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
})
    .WithTags("Todo")
    .Produces(201, typeof(TodoItem));

app.MapPut("/todoitems/{id}", async (int id, TodoItem todoItem, TodoDbContext db) =>
{
    if (id != todoItem.Id)
        return Results.BadRequest();

    if (!await db.TodoItems.AnyAsync(x => x.Id == id))
        return Results.NotFound();

    db.Update(todoItem);
    
    await db.SaveChangesAsync();

    return Results.Ok(todoItem);
})
    .WithTags("Todo")
    .Produces(200, typeof(TodoItem))
    .Produces(400)
    .Produces(404);

app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db) =>
{
    var todoItem = await db.TodoItems.FindAsync(id);

    if (todoItem is null)
        return Results.NotFound();

    db.TodoItems.Remove(todoItem);
    
    await db.SaveChangesAsync();
    
    return Results.NoContent();
})
    .WithTags("Todo")
    .Produces(204)
    .Produces(404);

app.Run();