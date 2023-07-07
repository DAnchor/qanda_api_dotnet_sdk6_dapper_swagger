using QandA.Api.Configuration;
using QandA.DataAccess.Configurations;
using QandA.DataAccess.Repositories;
using QandA.DTOs.Configuration;
using QandA.Security.Configuration;
using QandA.Services.Container;

var builder = WebApplication.CreateBuilder(args);

// DI registration container
// Repositories
builder.Services.AddScoped<IDataRepository, DataRepository>();
// Services
builder.Services.AddScoped<IQuestionsServiceContainer, QuestionsServiceContainer>();
builder.Services.AddScoped<IUserAuthServiceContainer, UserAuthServiceContainer>();
// Validators
builder.Services.AddValidationConfiguration();
// Memory cache
builder.Services.AddMemoryCacheConfiguration();
// Database configuration
builder.Services.AddDatabaseConfiguration(builder.Configuration);
// AutoMapper
builder.Services.AddMapperConfiguration();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Cors
builder.Services.AddCorsConfiguration(builder.Configuration);
// OAuth
builder.Services.AddOAuthConfiguration(builder.Configuration);
// Permissions
builder.Services.AddAuthorizationPoliciesConfiguration();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
