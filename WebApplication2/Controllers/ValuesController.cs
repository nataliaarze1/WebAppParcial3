using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using businesslogic.Models; // header added
using System.Threading.Tasks;
using businesslogic.Managers;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IPatientsManager _patientManager;

        public SampleController(IPatientsManager patientManager)//inyector de dependencias 
        {
            _patientManager = patientManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var students = _patientManager.GetAllPatients();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) // Use async Task<IActionResult>
        {
            var patient = await _patientManager.GetPatientByCI(id); // Use await
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
