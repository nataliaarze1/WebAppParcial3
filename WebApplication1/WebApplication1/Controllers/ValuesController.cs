using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using businesslogic.Models; // header added

namespace WebApplication1.Controllers
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
        public Patient Get(int id)
        {
            Task<Patient> patient = _patientManager.GetPatientByID(id);
            return patient.Result;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Patient patient)
        {
            if (patient == null)
            {
                return BadRequest("Invalid patient data");
            }
            var createdPatient = _patientManager.AddPatient(patient);
            return CreatedAtAction(nameof(Get), new { id = createdPatient.Id }, createdPatient);
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