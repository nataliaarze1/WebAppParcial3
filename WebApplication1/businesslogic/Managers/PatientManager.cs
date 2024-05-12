using businesslogic.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using businesslogic.Exceptions;
using businesslogic;
using Serilog;

namespace businesslogic.Managers
{
    public interface IPatientsManager
    {
        IEnumerable<Patient> GetAllPatients();
        Task<Patient> GetPatientByCI(int id);
        Patient AddPatient(Patient student);
        bool UpdatePatient(int id, Patient updatedStudent);
        bool DeletePatient(int id);
    }

    public class PatientManager : IPatientsManager
    {
        private readonly List<Patient> _patients = new List<Patient>();
        private readonly IConfiguration _configuration;
        private readonly FilePatientStorage _filePatientStorage;

        public PatientManager(IConfiguration configuration, FilePatientStorage filePatientStorage)
        {
            _configuration = configuration;
            _filePatientStorage = filePatientStorage;
            string filePath = _configuration.GetSection("FileStorage")["PatientDataFilePath"];
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("No se especificó la ubicación del archivo de datos de pacientes en la configuración.");
            }
            // Solo necesitas asignar _filePatientStorage una vez
            _filePatientStorage = new FilePatientStorage(Path.Combine(filePath, "patients.txt")); // Asegúrate de que la extensión del archivo sea ".txt"

            // Cargar pacientes desde el archivo al inicializar el PatientManager
            LoadPatientsFromFile();
        }

        private void LoadPatientsFromFile()
        {
            IEnumerable<string> patientData = _filePatientStorage.ReadPatientsFromFile();
            foreach (string line in patientData)
            {
                Patient patient = ParsePatientFromLine(line);
                _patients.Add(patient);
            }


        }

        private Patient ParsePatientFromLine(string line)
        {
            string[] parts = line.Split(',');
            return new Patient
            {
                Name = parts[0].Trim(),
                LastName = parts[1].Trim(),
                typeBlood = parts[2].Trim(),
                CI = int.Parse(parts[3].Trim()),
                PatientCode = parts[4].Trim()
            };
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return _patients;
        }

        public async Task<Patient> GetPatientByCI(int id)
        {
            var patient = _patients.FirstOrDefault(s => s.CI == id);
            if (patient == null)
            {
                throw new MyException("Patient not found");
            }
            return patient;
        }

        public Patient AddPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            // Generar CI si es el primer paciente o incrementar en uno el máximo CI existente
            patient.CI = _patients.Any() ? _patients.Max(s => s.CI) + 1 : 1;

            // Generar el código del paciente basado en el apellido y el CI
            string[] lastNameParts = patient.LastName.Split(' ');
            string patientCode = $"{lastNameParts[0].Substring(0, 1)}{lastNameParts[lastNameParts.Length - 1]}-{patient.CI}";

            // Asignar el código del paciente
            patient.PatientCode = patientCode;

            // Agregar el paciente a la lista de pacientes
            _patients.Add(patient);

            // Guardar el paciente recién agregado en el archivo
            _filePatientStorage.WritePatientsToFile(_patients.Select(p => $"{p.Name}, {p.LastName}, {p.typeBlood}, {p.CI}, {p.PatientCode}"));
            return patient;
        }

        public bool UpdatePatient(int id, Patient updatedPatient)
        {
            if (updatedPatient == null)
            {
                throw new ArgumentNullException(nameof(updatedPatient));
            }

            var existingPatient = _patients.FirstOrDefault(s => s.CI == id);
            if (existingPatient == null)
            {
                return false;
            }

            existingPatient.Name = updatedPatient.Name;

            // Guardar los pacientes en el archivo después de actualizar
            _filePatientStorage.WritePatientsToFile(_patients.Select(p => $"{p.Name}, {p.typeBlood}, {p.CI}"));

            return true;
        }

        public bool DeletePatient(int id)
        {
            var patientToRemove = _patients.FirstOrDefault(s => s.CI == id);
            if (patientToRemove == null)
            {
                return false;
            }

            _patients.Remove(patientToRemove);

            // Guardar los pacientes en el archivo después de eliminar uno
            _filePatientStorage.WritePatientsToFile(_patients.Select(p => $"{p.Name}, {p.typeBlood}, {p.CI}"));

            return true;
        }
    }
}
