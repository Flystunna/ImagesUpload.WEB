﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesUpload.WEB.Utils
{
    public interface IServiceResponse<T>
    {
        string Code { get; set; }
        string ShortDescription { get; set; }
        T Object { get; set; }
        bool IsValid { get; }
        Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }

    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public ServiceResponse(T response) : this()
        {
            Object = response;
        }

        public ServiceResponse()
        {
            ValidationErrors = new Dictionary<string, IEnumerable<string>>();
        }

        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public T Object { get; set; }

        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
        public bool IsValid { get => !ValidationErrors.Any() && Code == "200"; }
    }
}
