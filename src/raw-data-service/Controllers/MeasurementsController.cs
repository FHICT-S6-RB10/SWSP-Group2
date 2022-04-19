// //This class is only used for testing purposes
// using Raw_Data_Service.Models;
// using Raw_Data_Service.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace Raw_Data_Service.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class MeasurementsController : ControllerBase
// {
//     private readonly MeasurementsService _measurementsService;

//     public MeasurementsController(MeasurementsService MeasurementsService) =>
//         _measurementsService = MeasurementsService;

//     [HttpGet]
//     public async Task<List<Measurement>> Get() =>
//         await _measurementsService.GetAsync();

//     [HttpGet("{id:length(24)}")]
//     public async Task<ActionResult<Measurement>> Get(string id)
//     {
//         var Measurement = await _measurementsService.GetAsync(id);

//         if (Measurement is null)
//         {
//             return NotFound();
//         }

//         return Measurement;
//     }

//     [HttpPost]
//     public async Task<IActionResult> Post(Measurement newMeasurement)
//     {
//         Console.WriteLine("working...");
//         await _measurementsService.CreateAsync(newMeasurement);

//         return CreatedAtAction(nameof(Get), new { id = newMeasurement.Id }, newMeasurement);
//     }

//     [HttpPut("{id:length(24)}")]
//     public async Task<IActionResult> Update(string id, Measurement updatedMeasurement)
//     {
//         var Measurement = await _measurementsService.GetAsync(id);

//         if (Measurement is null)
//         {
//             return NotFound();
//         }

//         updatedMeasurement.Id = Measurement.Id;

//         await _measurementsService.UpdateAsync(id, updatedMeasurement);

//         return NoContent();
//     }

//     [HttpDelete("{id:length(24)}")]
//     public async Task<IActionResult> Delete(string id)
//     {
//         var Measurement = await _measurementsService.GetAsync(id);

//         if (Measurement is null)
//         {
//             return NotFound();
//         }

//         await _measurementsService.RemoveAsync(id);

//         return NoContent();
//     }
// }