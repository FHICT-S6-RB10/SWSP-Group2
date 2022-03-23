using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ServiceStateRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region HTTP requests
app.MapGet("/servicestates", ([FromServices] ServiceStateRepository repo) =>
{
    return repo.GetAll();
})
.WithName("GetServiceStates");

app.MapGet("/servicestatesmock", ([FromServices] ServiceStateRepository repo) =>
{
    var state1 = new ServiceState("Authentication", ServiceStatus.AVAILABLE);
    var state2 = new ServiceState("SensorDataStorage", ServiceStatus.HAS_ERRORS);
    repo.Create(state1);
    repo.Create(state2);
    return repo.GetAll();
})
.WithName("GetServiceStatesMock");

app.MapGet("/servicestates/{name}", ([FromServices] ServiceStateRepository repo, string name) =>
{
    var state = repo.GetByName(name);
    return state is not null ? Results.Ok(state) : Results.NotFound();
})
.WithName("GetServiceStateByName");

app.MapPost("/servicestates", ([FromServices] ServiceStateRepository repo, ServiceState state) =>
{
    repo.Create(state);
    return Results.Created($"/servicestates/{state.name}", state);
})
.WithName("CreateServiceStates");
#endregion

app.Run();

#region Data Management
internal record ServiceState(string name, ServiceStatus status);

enum ServiceStatus
{
    UNAVAILABLE = 0,
    AVAILABLE = 1,
    HAS_ERRORS = 2
}

class ServiceStateRepository
{
    private readonly List<ServiceState> _services = new List<ServiceState>();

    public void Create(ServiceState state)
    {
        if (state == null)
            return;

        _services.Add(state);
    }

    public List<ServiceState> GetAll()
    {
        return _services;
    }

    public ServiceState GetByName(string name)
    {
    #pragma warning disable CS8603 // Possible null reference return.
        return _services.Find(x => x.name == name);
    #pragma warning restore CS8603 // Possible null reference return.
    }

    public void Update(ServiceState state)
    {
        var existingState = GetByName(state.name);
        if(existingState == null)
            return;

        int index = _services.IndexOf(existingState);
        _services[index] = state;
    }
}
#endregion

