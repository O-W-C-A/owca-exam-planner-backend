using API.Data;
using API.Mapping;
using API.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Adaugă serviciul CORS pentru a permite accesul din orice domeniu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()  // Permite accesul din orice domeniu
               .AllowAnyMethod()  // Permite orice metodă HTTP (GET, POST, PUT, DELETE, etc.)
               .AllowAnyHeader(); // Permite orice header
    });
});

// Adaugă serviciile la container
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurarea conexiunii la baza de date
builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("APIConnectionString")));

// Adaugă mapping-ul și serviciile
builder.Services.AddScoped<CourseMapper>();
builder.Services.AddScoped<FacultyService>();

var app = builder.Build();

// Configurează pipeline-ul HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplică politica CORS
app.UseCors("AllowAll");
app.UseRouting();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
