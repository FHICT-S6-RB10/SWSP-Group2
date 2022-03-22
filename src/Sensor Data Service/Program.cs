//var app = WebApplication.CreateBuilder(args).Build();
//app.MapGet("/servicestates", () => 1);
//app.Run();

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<State>();

var app = builder.Build();

app.MapGet("/servicestates", () => new
{
    name = "SensorDataService",
    status = 1
});

app.Run();

record State(String name, int status);


















//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapRazorPages();

//app.Run();
