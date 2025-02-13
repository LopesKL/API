using API.Application.Dto.Request;
using System;
using System.Collections.Generic;

namespace API.Application.Dto
{
    public class HabilidadeDto
    {
        public Guid IdHabilidade { get; set; }
        public string Nome { get; set; }   // Exemplo: Figma, Support

        public ICollection<UsuarioHabilidadeDto> UsuarioHabilidadesDto { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllHabilidadeDto : RequestAllDto
    {
        public Guid IdHabilidade { get; set; }
        public string Nome { get; set; }   // Exemplo: Figma, Support

    }
}
