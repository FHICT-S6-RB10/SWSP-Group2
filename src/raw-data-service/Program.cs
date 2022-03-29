using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MeasurementRepository>();

var app = builder.Build();

app.MapGet("/measurements", ([FromServices] MeasurementRepository repo) => {
    return repo.GetAll();
});

app.MapGet("measurements/{id}", ([FromServices] MeasurementRepository repo, int id) =>{
    var measurement = repo.GetByUserId(id);
    return measurement is not null ? Results.Ok(measurement) : Results.NotFound();
});

app.MapPost("/measurements", ([FromServices] MeasurementRepository repo, Measurement measurement) => {
    repo.Create(measurement);
    return Results.Created($"/measurements/{measurement.id}", measurement);
});

app.Run();

record Measurement(int id, int userId, int stressLevel);

class MeasurementRepository {
    private readonly List<Measurement> _measurements = new List<Measurement>();

    public void Create(Measurement measurement){
        if (measurement is null){
            return;
        }

        _measurements.Add(measurement);
    }
   
    public List<Measurement> GetByUserId(int userId){        
        return _measurements.FindAll(x => x.userId == userId);       
    }
    public List<Measurement> GetAll(){
        return _measurements;
    }
}