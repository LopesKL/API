using Microsoft.AspNetCore.Http;

namespace API.Application.Dto.Request
{
    public class RequestAllDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortOrder { get; set; } // Campo a ser ordenado
        public string SorterField { get; set; } // Direção da ordenação (asc/desc)
    }

    public class FileUploadRequest
    {
        public IFormFileCollection Files { get; set; }
    }
}
