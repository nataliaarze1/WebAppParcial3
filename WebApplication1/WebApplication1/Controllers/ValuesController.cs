using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using businesslogic.Models; // Asegúrate de tener la referencia adecuada a tus modelos

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SampleController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5022/"); // Corrección: URL base de la API de respaldo
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/patients");
            if (response.IsSuccessStatusCode)
            {
                var patients = await response.Content.ReadAsAsync<IEnumerable<Patient>>();
                return Ok(patients);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al recuperar pacientes de la API de respaldo");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/patients/{id}");
            if (response.IsSuccessStatusCode)
            {
                var patient = await response.Content.ReadAsAsync<Patient>();
                return Ok(patient);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al recuperar paciente de la API de respaldo");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] Patient patient)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/patients", patient);
            if (response.IsSuccessStatusCode)
            {
                var createdPatient = await response.Content.ReadAsAsync<Patient>();
                return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.Id }, createdPatient);
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al agregar paciente en la API de respaldo");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/patients/{id}", updatedPatient);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al actualizar paciente en la API de respaldo");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/patients/{id}");
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al eliminar paciente en la API de respaldo");
            }
        }
    }
}