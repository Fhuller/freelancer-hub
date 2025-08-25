using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para o console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger com Authorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Freelancer API", Version = "v1" });

    // Adicionar configuração de Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configurar CORS para localhost e frontend hospedado
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://freelancer-hub-tau.vercel.app"
            )
            .AllowAnyHeader() // permite Authorization, Content-Type etc.
            .AllowAnyMethod() // GET, POST, PUT, DELETE, OPTIONS
            .AllowCredentials(); // caso você use cookies
    });
});

// Configurar EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FreelancerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthorization();

var keyBytes = Encoding.UTF8.GetBytes("r0kxY3QfjQQBgZbqxspFmHiVZFAAFEMeocxVMp6r/F9jtvaa5ESMF1AG+rZDoWNuD7TKRzgmFXqr/NhLn56iEg==");

builder.Services.AddAuthentication()
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidIssuer = "https://ccsaysezfijfkydhldow.supabase.co/auth/v1",
            ValidAudience = "authenticated",
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

var app = builder.Build();

// Usar CORS antes de mapear endpoints e antes de autenticação
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FreelancerContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.Migrate();
        logger.LogInformation("Migrations aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Falha ao aplicar migrations no banco de dados.");
        throw;
    }
}

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Freelancer API V1");
});

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Falha ao iniciar a aplicação");
    throw;
}
