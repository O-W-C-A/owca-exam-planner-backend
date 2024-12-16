using API.Data;
using API.Mapping;
using API.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add CORS policy to allow requests from any domain
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()  // Allow any origin
                     .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
                     .AllowAnyHeader(); // Allow any header
    });
});

// Add services to the DI container
builder.Services.AddControllers(); // Add controllers
builder.Services.AddEndpointsApiExplorer(); // Add API explorer for Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // Add Swagger/OpenAPI generator

// Configure database connection
var connectionString = Environment.GetEnvironmentVariable("APIConnectionString")
    ?? builder.Configuration.GetConnectionString("APIConnectionString");

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(connectionString)); // Use SQL Server

// Register custom services and mappers
builder.Services.AddScoped<CourseMapper>();  // Add CourseMapper as scoped service
builder.Services.AddScoped<FacultyService>(); // Add FacultyService as scoped service

// Configure JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Ensure the token has a valid signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])), // Signing key
            ValidateIssuer = false, // Skip issuer validation
            ValidateAudience = false // Skip audience validation
        };
    });

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy to allow cross-origin requests
app.UseCors("AllowAll");

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable routing
app.UseRouting();

// Add middleware for authentication and authorization
app.UseAuthentication(); // Process JWT authentication
app.UseAuthorization();  // Enforce authorization policies

// Map controller routes
app.MapControllers();

// Run the application
app.Run();
