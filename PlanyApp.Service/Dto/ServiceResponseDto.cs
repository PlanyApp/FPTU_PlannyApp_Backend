using System.Collections.Generic;

namespace PlanyApp.Service.Dto
{
    public class ServiceResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ServiceResponseDto(bool success = false, string? message = null, T? data = default, List<string>? errors = null)
        {
            IsSuccess = success;
            Message = message;
            Data = data;
            if (errors != null)
            {
                Errors = errors;
            }
        }
    }
} 