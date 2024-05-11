using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace businesslogic.Models
{
    public class Patient
    {
        public int CI { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string typeBlood { get; set; }
        public string PatientCode { get; set; }

        

    }
}
