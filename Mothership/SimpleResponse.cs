using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MothershipLib
{
    class SimpleResponse
    {
        public enum ResultType
        {
            OK,
            ERROR
        }

        public string result = ResultType.OK.ToString();
        public string message = "";

        public SimpleResponse(ResultType type)
        {
            this.result = type.ToString();
        }

        public SimpleResponse(ResultType type, string message)
        {
            this.result = type.ToString();
            this.message = message;
        }
    }
}
