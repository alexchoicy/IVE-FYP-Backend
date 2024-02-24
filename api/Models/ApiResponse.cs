using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.utils
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; } = 200;
    }
}