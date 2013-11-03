using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractiveCsApi.Models
{
    public class Code
    {
        public bool Success { get; set; }
        public string CurrentCode { get; set; }
        public string Message { get; set; }
        public IList<string> Buffer { get; set; }
    }
}