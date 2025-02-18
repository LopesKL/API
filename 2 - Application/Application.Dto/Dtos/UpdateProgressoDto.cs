using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Application.Dto.Request;

namespace API.Application.Dto
{
    public class UpdateProgressoDto
    {
        public Guid IdAtividade { get; set; }
        public int Progresso { get; set; }
    }

}
