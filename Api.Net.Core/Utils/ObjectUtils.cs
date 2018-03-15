using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class ObjectUtils
    {
        public static T EmptyIfNull<T>(this T obj)
        {
            var stringType = typeof(string);
            var properties = obj.GetType().GetTypeInfo().DeclaredProperties;
            properties = properties.Where(t => t.PropertyType.Equals(stringType));
            foreach (var property in properties)
            {
                if (property.GetValue(obj) == null) property.SetValue(obj, string.Empty);
            }
            return obj;
        }

        public static string GetInnerMessages(this Exception ex)
        {
            StringBuilder messages = new StringBuilder();
            messages.AppendLine(ex.Message);
            Exception _innerExcepcion = ex.InnerException;

            if (ex is DbEntityValidationException)
            {
                DbEntityValidationException dbEx = (DbEntityValidationException)ex;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        messages.AppendLine(message);
                    }
            }
            else
            {
              while (_innerExcepcion != null)
                {
                    messages.AppendLine(_innerExcepcion.Message);
                    _innerExcepcion = _innerExcepcion.InnerException;
                }
            }
            return messages.ToString();
        }
    }
}
