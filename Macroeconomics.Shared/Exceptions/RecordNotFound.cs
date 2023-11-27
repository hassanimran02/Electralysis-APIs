using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.Shared.Exceptions
{
    public class RecordNotFound : Exception
    {
        public RecordNotFound() : base() { }
        public RecordNotFound(string message) : base(message) { }
        public RecordNotFound(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
