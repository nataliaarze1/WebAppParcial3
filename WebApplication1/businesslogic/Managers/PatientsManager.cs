using System;
using System.Collections.Generic;
using System.Linq;
using businesslogic.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using Serilog;

public interface IPatientsManager
{
    IEnumerable<Patient> GetAllPatients();
    Task<Patient> GetPatientByCI(int id);
    Patient AddPatient(Patient student);
    bool UpdatePatient(int id, Patient updatedStudent);
    bool DeletePatient(int id);
}

public class StudentManager : IPatientsManager
{
    private readonly List<Patient> _patients = new List<Patient>();
    private readonly IConfiguration _configuration;

    public StudentManager(IConfiguration configuration)
    {
        _configuration = configuration;
        _patients = new List<Patient>
        {
            new Patient { Id = 1, Name = "Mario Torrez" },
            new Patient { Id = 2, Name = "Natalia Arze" },
            new Patient { Id = 3, Name = "Juan Perez" }
        };
    }

    public IEnumerable<Patient> GetAllPatients()
    {
        return _patients;
    }
    public List<Patient> GetAll()
    {
        string connetionString = _configuration.GetSection("ConnectionStrings").GetSection("dbConnection").Value;
        Console.WriteLine(connetionString);
        return _patients;
    }

    public async Task<Patient> GetPatientByCI(int id)
    {
       HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync("https://api.restful-api.dev/objects/" + id);
        string responsebody = await response.Content.ReadAsStringAsync();
        Log.Information("Response: {0}", responsebody);
        Patient foundStudent = _patients.Find(s => s.Id == id);
        return foundStudent;
    }

    public Patient AddPatient(Patient student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        student.Id = _patients.Any() ? _patients.Max(s => s.Id) + 1 : 1;
        _patients.Add(student);
        return student;
    }

    public bool UpdatePatient(int id, Patient updatedStudent)
    {
        if (updatedStudent == null)
        {
            throw new ArgumentNullException(nameof(updatedStudent));
        }

        var existingStudent = _patients.FirstOrDefault(s => s.Id == id);
        if (existingStudent == null)
        {
            return false;
        }

        existingStudent.Name = updatedStudent.Name;
        return true;
    }

    public bool DeletePatient(int id)
    {
        var studentToRemove = _patients.FirstOrDefault(s => s.Id == id);
        if (studentToRemove == null)
        {
            return false;
        }

        _patients.Remove(studentToRemove);
        return true;
    }
}