using Company.AppOne.Db;
using Company.Common.EfCore;
using Company.Common.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddCommonApi();
builder.AddOneDbContext();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapCommonEndpoints();

// app.MapGet("/foos", () => TypedResults.Ok());
// app.MapGet("/foos/{id}", () => TypedResults.Ok());
// app.MapPost("/foos", () => TypedResults.Ok());
// app.MapPut("/foos/{id}", () => TypedResults.Ok());
// app.MapDelete("/foos/{id}", () => TypedResults.Ok());

await using var scope = app.Services.CreateAsyncScope();
var oneDbContext = scope.ServiceProvider.GetRequiredService<OneDbContext>();
await oneDbContext.Database.EnsureDeletedAsync();
await oneDbContext.Database.EnsureCreatedAsync();
_ = await oneDbContext.Foos.ToListAsync();

app.Run();
