using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Utilities.Exceptions
{
    public class ESMSException : Exception
    {
        public ESMSException()
        {
        }

        public ESMSException(string message)
            : base(message)
        {
        }

        public ESMSException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}