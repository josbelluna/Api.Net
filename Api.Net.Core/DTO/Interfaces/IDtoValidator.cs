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
        void ValidateInsert(Validator<TDto> validator);
        void ValidateUpdate(Validator<TDto> validator);
        void ValidateSave(Validator<TDto> validator);
    }
}
