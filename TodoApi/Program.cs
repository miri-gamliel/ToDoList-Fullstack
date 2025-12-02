using TodoApi;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()    
              .AllowAnyMethod()   
              .AllowAnyHeader();   
    });
});

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString))
     .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));


var app = builder.Build();

app.UseCors("AllowAll");
// Healthcheck endpoint
app.MapGet("/ping", () => Results.Ok("pong"));

app.MapGet("/todos", (ToDoDbContext db) => 
{
     return db.Items.ToList();
});

app.MapPost("/todos",(CreateItemDto item,ToDoDbContext db)=>
{
    Item newItem=new Item{Name=item.Name,IsComplete=item.IsComplete};
    db.Items.Add(newItem);
    db.SaveChanges();
    return Results.Created($"/todos/{newItem.Id}", newItem);
});

app.MapPut("/todo/{id}",(int id,bool IsComplete,ToDoDbContext db)=>
{
    var item=db.Items.Find(id);
    if(item==null)
        return Results.NotFound();
    item.IsComplete=IsComplete;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("/todo/{id}",(int id,ToDoDbContext db)=>
{
    var item=db.Items.Find(id);
    if(item==null)
        return Results.NotFound();
    db.Items.Remove(item);    
    db.SaveChanges();
    return Results.Ok(item);
});
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    // In development, redirect root to Swagger UI
//}
//else
//{
    // In production, return a simple message on root
    app.MapGet("/", () => Results.Ok("API is running"));
//}
app.Run();
