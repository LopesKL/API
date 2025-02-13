using API.Application.Dto.Request;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;

namespace API.Application.Dto {
    public class AtividadeDto {
        public Guid IdAtividade { get; set; }
        public Guid IdAtividadeFilho { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Status { get; set; }
        public int Progresso { get; set; }
        public int HorasEstimadas { get; set; }
        public int HorasTotais { get; set; }
        public bool Cobrado { get; set; }
        public string Descricao { get; set; }
        public string Files { get; set; }

        public AtividadeFilhoDto AtividadeFilhoDto { get; set; }
        public ICollection<AtividadeUsuarioDto> AtividadeUsuarioDto { get; set; }
        public ICollection<ComentarioDto> Comentario { get; set; }


        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllAtividadeDto : RequestAllDto {
        public Guid IdAtividade { get; set; }
        public Guid IdAtividadeFilho { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Status { get; set; }
        public int Progresso { get; set; }
        public int HorasEstimadas { get; set; }
        public int HorasTotais { get; set; }
        public bool Cobrado { get; set; }
        public string Descricao { get; set; }
        public string Files { get; set; }

        public AtividadeFilhoDto AtividadeFilhoDto { get; set; }
        public ICollection<AtividadeUsuarioDto> AtividadeUsuarioDto { get; set; }
        public ICollection<ComentarioDto> Comentario { get; set; }


    }
}
