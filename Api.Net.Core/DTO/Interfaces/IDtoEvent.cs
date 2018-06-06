using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Dto.Interfaces
{
    public interface IDtoEvent<TDto> where TDto : class
    {
       void BeforeGet(TDto dto);
       void BeforeInsert(TDto dto);
       void AfterInsert(TDto dto);
       void BeforeUpdate(TDto dto);
       void AfterUpdate(TDto dto);
       void BeforeSave(TDto dto);
       void AfterSave(TDto dto);   
    }
}
