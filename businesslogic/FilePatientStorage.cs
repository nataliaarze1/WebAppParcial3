using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace businesslogic
{
    public class FilePatientStorage
    {

        private readonly string _filePath;

        public FilePatientStorage(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<string> ReadPatientsFromFile()
        {
            List<string> patientsData = new List<string>();

            try
            {
                if (File.Exists(_filePath))
                {
                    using (StreamReader reader = new StreamReader(_filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            patientsData.Add(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                // Log de errores de acceso al archivo
                Log.Error(ex, "Error de acceso al archivo: {Message}", ex.Message);
                throw; // Re-lanzar la excepción para que sea manejada por el cliente
            }
            catch (Exception ex)
            {
                // Log de otros errores
                Log.Error(ex, "Error inesperado al leer el archivo: {Message}", ex.Message);
                throw; // Re-lanzar la excepción para que sea manejada por el cliente
            }

            return patientsData;
        }

        public void WritePatientsToFile(IEnumerable<string> patientsData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    foreach (string patientData in patientsData)
                    {
                        writer.WriteLine(patientData);
                    }
                }
            }
            catch (IOException ex)
            {
                // Log de errores de acceso al archivo
                Log.Error(ex, "Error de acceso al archivo: {Message}", ex.Message);
                throw; // Re-lanzar la excepción para que sea manejada por el cliente
            }
            catch (Exception ex)
            {
                // Log de otros errores
                Log.Error(ex, "Error inesperado al escribir en el archivo: {Message}", ex.Message);
                throw; // Re-lanzar la excepción para que sea manejada por el cliente
            }
        }
    
    }
}
