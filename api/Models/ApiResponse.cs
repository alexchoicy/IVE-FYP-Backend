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

    public class PagedResponse<T>
    {
        public required int CurrentPage { get; set; }
        public required int PageSize { get; set; }
        public required int TotalCount { get; set; }
        public required int TotalPages { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
        public T? Data { get; set; }
    }
}