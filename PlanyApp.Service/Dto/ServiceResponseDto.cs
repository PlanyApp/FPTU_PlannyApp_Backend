using System.Collections.Generic;

namespace PlanyApp.Service.Dto
{
    public class ServiceResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ServiceResponseDto(bool success = false, string? message = null, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            if (errors != null)
            {
                Errors = errors;
            }
        }
    }
} 