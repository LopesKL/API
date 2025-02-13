using System;
using System.Collections.Generic;

namespace API.Domain.Projeto
{
    public class Lancamento
    {
        public Guid IdLancamento { get; set; }
        public Guid IdProjeto { get; set; }
        public Guid? IdAtividadePai { get; set; }
        public Guid? IdAtividadeFilho { get; set; }
        public Guid? idTag { get; set; }
        public Guid IdUsuario { get; set; }
        public string Descricao { get; set; }
        public DateTime Data {  get; set; }
        public DateTime HorarioInicio { get; set; }
        public DateTime HorarioFim { get; set; }
        public int Horas { get; set; }
        public bool Cobrado { get; set; }
        public int Valor {  get; set; }

        public Projetos Projetos { get; set; }
        public AtividadePai AtividadePai { get; set; }
        public AtividadeFilho AtividadeFilho { get; set; }
        public Tag Tag { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
