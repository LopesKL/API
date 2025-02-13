using API.Application.Dto.Request;
using Azure.Storage.Blobs.Models;
using System;

namespace API.Application.Dto
{
    public class TagDto
    {
        public Guid IdTag { get; set; }
        public string Nome { get; set; }   // Ex: Suporte Thrive, Suporte LEC... 


        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllTagDto : RequestAllDto
    {
        public Guid IdTag { get; set; }
        public string Nome { get; set; }   // Ex: Suporte Thrive, Suporte LEC... 
    }
}
