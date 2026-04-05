using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options => options.AddScalarTransformers());
builder.Services.Configure<ScalarOptions>(options => options.DisableAgent());

var app = builder.Build();

app.UseHttpsRedirection();

app.MapOpenApi();
app.MapScalarApiReference();

// app.MapGet("/bars", () => TypedResults.Ok());
// app.MapGet("/bars/{id}", () => TypedResults.Ok());
// app.MapPost("/bars", () => TypedResults.Ok());
// app.MapPut("/bars/{id}", () => TypedResults.Ok());
// app.MapDelete("/bars/{id}", () => TypedResults.Ok());

app.Run();

