using BatchPay.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder( args );

// 🔹 Tilføj DbContext (Entity Framework Core)
builder.Services.AddDbContext<BatchPayContext>( options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

// 🔹 Tilføj CORS policy
builder.Services.AddCors( options =>
{
    options.AddPolicy( "AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    } );
} );

// 🔹 Controllers og Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Brug CORS
app.UseCors( "AllowAll" );

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
