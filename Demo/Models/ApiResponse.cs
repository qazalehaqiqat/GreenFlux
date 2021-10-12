using System;
using System.Collections.Generic;

namespace Demo.Models
{
    public class APIResponse<T>
    {
        public bool Succeeded { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
