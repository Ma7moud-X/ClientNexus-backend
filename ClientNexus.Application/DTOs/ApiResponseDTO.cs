using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponseDTO() { }

        public ApiResponseDTO(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponseDTO<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponseDTO<T>(true, message, data);
        }

        public static ApiResponseDTO<T> ErrorResponse(string message)
        {
            return new ApiResponseDTO<T>(false, message, default);
        }
    }
}
