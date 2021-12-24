

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
 {
     config.Sources.Clear();

     var env = hostingContext.HostingEnvironment;

     config.SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                          optional: true, reloadOnChange: true);

     config.AddEnvironmentVariables();

     if (args != null)
     {
         config.AddCommandLine(args);
     }
 });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSignalR()
    .AddMessagePackProtocol();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<RelayBoardHub>("/RelayBoardHub");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapPost("/api/account/signup", async (LoginViewModel model, [FromServices] ApplicationDbContext context, [FromServices] SignInManager<IdentityUser> signInManager) =>
{
    if (model == null || model.UserName.IsNullOrWhiteSpace() || model.Password.IsNullOrWhiteSpace())
    {
        return Results.Json(new ApiResponse(false, "ورودی (ها) خالی است"));
    }

    var user = context.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
    if (user == null)
    {
        return Results.Json(new ApiResponse(false, "کاربر یافت نشد"));
    }

    var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
    if (result.Succeeded)
    {
        return Results.Json(new ApiResponse(true));
    }
    else if (!result.Succeeded)
    {
        return Results.Json(new ApiResponse(false, "رمز عبور وارد شده نادرست است"));
    }
    else if (result.IsNotAllowed)
    {
        return Results.Json(new ApiResponse(false, "حساب کاربری شما غیر فعال شده است"));
    }
    else if (result.IsLockedOut)
    {
        return Results.Json(new ApiResponse(false, "بدلیل ورود بیش از حد رمز اشتباه این اکانت به صورت موقت مسدود گردید. لطفا کمی بعد مجددا تلاش کنید"));
    }

    return Results.Json(new ApiResponse(false, "خطا!"));
}).WithName("Login");

app.Run();

Task.Run(async () =>
{
    try
    {
        await DataBaseSeed.Initialize(app.Services);
    }
    catch (Exception)
    {
    }
});

internal record WeatherForecast(DateTime Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}