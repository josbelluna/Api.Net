using System;
using Api.Parameters;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Api.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Api;
using Api.Attributes;

namespace Api.Controllers
{
    [ApiControllerModelConvention]
    public class ApiController<TDto> : Controller where TDto : class
    {
        public string ApplicationUser => Request.Headers["ApplicationUser"];    
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Service =  HttpContext.RequestServices.GetService<IService<TDto>>();
            ListService = HttpContext.RequestServices.GetService<IListService>();
        }
        protected IService<TDto> Service { get; set; }
        protected IListService ListService { get; set; }

        [HttpGet]
        [Route("{id:int}")]
        public virtual IActionResult Ver(int id)
        {
            try
            {
                TDto _obj = Service.Find(id);

                if (_obj == null)
                    throw new ValidateException("The specified resource was not fout");

                return Ok(_obj);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public virtual IActionResult GetAll(ApiParameter parameter)
        {
            try
            {
                var servicioList = Service.Dto;
                var parameters = parameter.ProcessParameters(Request.Query);
                parameters.Filters.Add("Estado", true);
                var list = ListService.GetList(servicioList, parameters);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPost]
        public virtual IActionResult Add([FromBody] TDto _obj)
        {
            try
            {
                var type = _obj.GetType();

                type.GetProperty("FechaCreacion")?.SetValue(_obj, DateTime.Now);
                type.GetProperty("CreadoPor")?.SetValue(_obj, ApplicationUser ?? "SYSTEM");

                var version = type.GetProperty("Version");
                version?.SetValue(_obj, ((int)version.GetValue(_obj)) + 1);
                _obj = Service.Add(_obj);
                return Ok(_obj);
            }
            catch (ValidateException ex)
            {
                Response.ContentType = "text/plain";
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public virtual IActionResult Put([FromBody] TDto _obj)
        {
            try
            {
                var type = _obj.GetType();

                type.GetProperty("FechaModificacion")?.SetValue(_obj, DateTime.Now);
                type.GetProperty("ModificadoPor")?.SetValue(_obj, ApplicationUser ?? "SYSTEM");

                var version = type.GetProperty("Version");
                version?.SetValue(_obj, ((int)version.GetValue(_obj)) + 1);

                _obj = Service.Update(_obj);
                return Ok(_obj);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }


        [HttpPatch]
        [Route("{id:int}")]
        public virtual IActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<TDto> patch)
        {
            try
            {
                var _obj = Service.Find(id);
                patch.ApplyTo(_obj);

                var type = _obj.GetType();

                type.GetProperty("FechaModificacion")?.SetValue(_obj, DateTime.Now);
                type.GetProperty("ModificadoPor")?.SetValue(_obj, ApplicationUser);

                var version = type.GetProperty("Version");
                version?.SetValue(_obj, ((int)version.GetValue(_obj)) + 1);

                Service.Update(_obj);
                return Ok(_obj);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public virtual IActionResult Delete(int id)
        {
            try
            {
                var result = Service.Delete(id, ApplicationUser);
                return Ok(result);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }
    }
}
