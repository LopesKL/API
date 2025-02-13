using API.Application.Dto;
using System;
using System.Collections.Generic;

namespace API.Domain.Projeto {
    public class Atividade {
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
        public string Files {  get; set; }

        public AtividadeFilho AtividadeFilho { get; set; }
        public ICollection<AtividadeUsuario> AtividadeUsuario { get; set; }
        public ICollection<Comentario> Comentario { get; set; }


        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
