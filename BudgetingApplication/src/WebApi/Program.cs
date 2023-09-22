using Application;
using Infrastructure;
using Infrastructure.Persistence;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId(builder.Configuration["Authentication:Schemes:Bearer:ClientId"]);
        options.OAuthUsePkce();
        options.OAuthAppName("Budgeting Demo App");
    });
    
    // Initialise and seed database
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.InitialiseAsync();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();