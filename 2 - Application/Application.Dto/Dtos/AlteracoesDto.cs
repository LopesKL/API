using API.Application.Dto.Request;
using System;
using System.Text.Json;

namespace API.Application.Dto
{
    public class AlteracoesDto {
        public Guid IdAlteracao { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime DataEHora { get; set; }
        public string NomeTabela { get; set; }
        public string TipoOperacao { get; set; }
        public string ValoresAntigos { get; set; }
        public string ValoresNovos { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        // Métodos para converter objetos para JSON (mantendo a consistência com a entidade Alteracoes)
        public void SetValoresAntigos(object valores) {
            ValoresAntigos = JsonSerializer.Serialize(valores);
        }

        public void SetValoresNovos(object valores) {
            ValoresNovos = JsonSerializer.Serialize(valores);
        }

        public T GetValoresAntigos<T>() {
            return string.IsNullOrEmpty(ValoresAntigos) ? default : JsonSerializer.Deserialize<T>(ValoresAntigos);
        }

        public T GetValoresNovos<T>() {
            return string.IsNullOrEmpty(ValoresNovos) ? default : JsonSerializer.Deserialize<T>(ValoresNovos);
        }
    }

        public class RequestAllAlteracoesDto : RequestAllDto {
            public Guid IdAlteracao { get; set; }
            public Guid IdUsuario { get; set; }
            public DateTime DataEHora { get; set; }
            public string NomeTabela { get; set; }
            public string TipoOperacao { get; set; }
            public string ValoresAntigos { get; set; }
            public string ValoresNovos { get; set; }

            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? Updated { get; set; }
            public Guid CreatedBy { get; set; }
            public Guid? UpdatedBy { get; set; }
            public bool IsDeleted { get; set; }
        }
    }

