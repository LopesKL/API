using API.Application.Dto.Request;
using Azure;
using Azure.Storage.Blobs.Models;
using System;

namespace API.Application.Dto
{
    public class LancamentoDto {
        public Guid IdLancamento { get; set; }
        public Guid IdProjeto { get; set; }
        public Guid? IdAtividadePai { get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid IdAtividade { get; set; }

        public DateTime Dia { get; set; }

        public Guid? idTag { get; set; }
        public Guid IdUsuario { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public DateTime? HorarioInicio { get; set; }
        public DateTime? HorarioFim { get; set; }
        public string Horas { get; set; }
        public bool Cobrado { get; set; }
        public int Valor { get; set; }

        public ProjetosDto Projetos { get; set; }
        public AtividadePaiDto AtividadePai { get; set; }
        public AtividadeFilhoDto AtividadeFilho { get; set; }
        public TagDto Tag { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    // herda as props do requestAllDto
    public class RequestAllLancamentoDto : RequestAllDto
    {
        public Guid IdLancamento { get; set; }
        public Guid IdProjeto { get; set; }
        public Guid? IdAtividadePai { get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid? idTag { get; set; }
        public Guid IdUsuario { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public DateTime HorarioInicio { get; set; }
        public DateTime HorarioFim { get; set; }
        public int Horas { get; set; }
        public bool Cobrado { get; set; }
        public int Valor { get; set; }

        public ProjetosDto Projetos { get; set; }
        public AtividadePaiDto AtividadePai { get; set; }
        public AtividadeFilhoDto AtividadeFilho { get; set; }
        public TagDto Tag { get; set; }
    }
}
