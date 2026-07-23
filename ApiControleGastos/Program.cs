using ApiControleGastos.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o contexto SQLite (arquivo de banco de dados local 'controle_gastos.db')
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=controle_gastos.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS - Permite qualquer origem/porta (necessário para comunicação Front->Back)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Garantir que a política de CORS seja aplicada ANTES dos controllers
app.UseCors("AllowAll");

// Cria o banco SQLite e as tabelas na primeira execução
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();