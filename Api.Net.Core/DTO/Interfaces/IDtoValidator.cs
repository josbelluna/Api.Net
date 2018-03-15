using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Dto.Interfaces
{
    public interface IDtoValidator<TDto>
    {
        void ValidateInsert(TDto dto, Error error);
        void ValidateUpdate(TDto dto, Error error);
        void ValidateSave(TDto dto, Error error);
    }
}
