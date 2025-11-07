using ApiTesteRisePay.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra RisePayService com HttpClient e BaseAddress
builder.Services.AddHttpClient<RisePayService>(c =>
{
    c.BaseAddress = new Uri("https://api.risepay.com.br/api/external/Transactions");
});

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Permite arquivos estáticos da pasta wwwroot
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
