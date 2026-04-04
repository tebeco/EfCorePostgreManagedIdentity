using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options => options.AddScalarTransformers());
builder.Services.Configure<ScalarOptions>(options => options.DisableAgent());

var app = builder.Build();

app.UseHttpsRedirection();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/foos", () => TypedResults.Ok());
app.MapGet("/foos/{id}", () => TypedResults.Ok());
app.MapPost("/foos", () => TypedResults.Ok());
app.MapPut("/foos/{id}", () => TypedResults.Ok());
app.MapDelete("/foos/{id}", () => TypedResults.Ok());

app.Run();

