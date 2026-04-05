using Company.Common.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCommonApi();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapCommonEndpoints();

// app.MapGet("/bars", () => TypedResults.Ok());
// app.MapGet("/bars/{id}", () => TypedResults.Ok());
// app.MapPost("/bars", () => TypedResults.Ok());
// app.MapPut("/bars/{id}", () => TypedResults.Ok());
// app.MapDelete("/bars/{id}", () => TypedResults.Ok());

app.Run();

