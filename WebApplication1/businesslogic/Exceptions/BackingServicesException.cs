using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businesslogic.Exceptions
{
    public class BackingServicesException:Exception
    {
        public BackingServicesException(string message) : base(message)
        {

        }
        public string GetExceptionMessage(string methodName)
        {
            return methodName + " failed with message: " + Message;
        }
    }
}
