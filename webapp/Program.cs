using System.Diagnostics;
using ContractPortal;
using ContractPortal.Services;
using SockNet.ClientSocket;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);
string port = Environment.GetEnvironmentVariable("PORT");
// Hardcoded for demo purpose
string JWT_SECRET_KEY = "contract_portal_web_jwt_secret_key";


Process process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "bash",
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    }
};
process.Start();

// Add services to the container.
builder.Services.AddSingleton<Process>(process);
var socketClient = new SocketClient("127.0.0.1", 6900) ;
builder.Services.AddSingleton<SocketClient>(socketClient);
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

// configure strongly typed settings object
builder.Services.AddSingleton<AppSettings>(new AppSettings { Secret = JWT_SECRET_KEY });
builder.Services.AddScoped<IUserService, UserService>();

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapFallbackToFile("index.html"); ;

app.Run();
