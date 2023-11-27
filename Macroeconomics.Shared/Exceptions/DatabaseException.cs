using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string Message)
            : base(Message)
        {

        }
        public DatabaseException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
