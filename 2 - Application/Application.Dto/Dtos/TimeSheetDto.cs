using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Application.Dto.Request;

namespace API.Application.Dto
{
    public class TimeSheetDto
    {
        public string Key { get; set; }
        public string Projeto { get; set; }
        public string TarefaPai { get; set; }
        public string TarefaFilha { get; set; }
        public string Tarefa { get; set; }
        public int Progresso { get; set; }
        public string Cor { get; set; }

        public Guid IdAtividade { get; set; }
        public Dictionary<string, decimal> Horas { get; set; }
    }

}
