using System;
using System.Collections.Generic;
using System.Linq;
using businesslogic.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using businesslogic.Exceptions;
using businesslogic.Exceptions;

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
            new Patient { Id = 1, Name = "Maria Bolio", typeBlood = "O+"},
            new Patient { Id = 2, Name = "Priscilla Arias", typeBlood = "O+" },
            new Patient { Id = 3, Name = "Mia Colucci", typeBlood = "A- "},
            new Patient { Id = 4, Name = "Fernanda Martin", typeBlood = "A- "}

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
        var patient = _patients.FirstOrDefault(s => s.Id == id);
        if (patient == null)
        {
            throw new BackingServicesException("Patient not found");
        }
        return patient;

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