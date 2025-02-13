using System;
using System.Collections.Generic;

namespace API.Domain.Projeto
{
    public class Projetos
    {
        public Guid IdProjetos { get; set; }
        public Guid IdEmpresa { get; set; }
        public Guid IdCliente { get; set; }
        public string Nome { get; set; }
        public int OrcamentoInicial { get; set; }
        public int Gastos {  get; set; }
        public int SaldoFinal { get; set; }
        public string Moeda { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int HorasEstimadas { get; set; }
        public int HorasCobradas { get; set; }
        public int HorasNaoCobradas { get; set; }
        public string Cor { get; set; }
        public string Status { get; set; }
        public bool ProjetoConcluido { get; set; }
        public string Descricao { get; set; }
        public string Files { get; set; }

        public ICollection<ProjetoUsuario> ProjetoUsuario { get; set; }
        public ICollection<AtividadePai> AtividadePai { get; set; }
        public ICollection<Comentario> Comentario { get; set; }
        public ICollection<Lancamento> Lancamento { get; set; }

        public Empresa Empresa { get; set; }
        public Cliente Cliente { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
