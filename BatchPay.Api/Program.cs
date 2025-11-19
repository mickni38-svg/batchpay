using BatchPayLogic;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder( args );

// EF
builder.Services.AddDbContext<BatchPayContext>( opt =>
    opt.UseSqlServer( builder.Configuration.GetConnectionString( "BatchPay" ),
        b => b.MigrationsAssembly( "Data" ) )
);

builder.Services.AddScoped<IServiceLogic, ServiceLogic>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // VIGTIG
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc( "v1", new OpenApiInfo
    {
        Title = "BatchPay API",
        Version = "v1",
        Description = "API til globale brugere og venneliste"
    } );
} );

var app = builder.Build();

// Swagger (lad det være aktivt i Dev)
app.UseSwagger(); // svarer på /swagger/v1/swagger.json
app.UseSwaggerUI( c =>
{
    c.SwaggerEndpoint( "/swagger/v1/swagger.json", "BatchPay API v1" );
    c.RoutePrefix = "swagger"; // UI på /swagger
} );

// (valgfrit) app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
