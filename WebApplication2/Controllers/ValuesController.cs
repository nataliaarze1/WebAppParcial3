using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using businesslogic.Models; // Asegúrate de tener la referencia adecuada a tus modelos
using System.Threading.Tasks;
using businesslogic.Managers;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IPatientsManager _patientManager;

        public SampleController(IPatientsManager patientManager)
        {
            _patientManager = patientManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var patients = _patientManager.GetAllPatients();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var patient = await _patientManager.GetPatientByCI(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Invalid patient data");
            }
            var createdPatient = _patientManager.AddPatient(patient);
            return CreatedAtAction(nameof(Get), new { id = createdPatient.CI }, createdPatient);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Patient updatedPatient)
        {
            var result = _patientManager.UpdatePatient(id, updatedPatient);
            if (!result)
            {
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _patientManager.DeletePatient(id);
            if (!result)
            {
                return NotFound("Patient not found");
            }
            return NoContent();
        }
    }
}