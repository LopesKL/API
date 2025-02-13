using System;
using System.Collections.Generic;

namespace API.Domain.Projeto
{
    public class Tag
    {
        public Guid IdTag { get; set; }
        public string Nome { get; set; }   // Ex: Suporte Thrive, Suporte LEC... 

        public ICollection<Lancamento> Lancamento { get; set; }

        // controls
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
