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
using Api.Net.Core.Utils;
using Api.Net.Core.Metatada;

namespace Api.Controllers
{
    [ApiControllerModelConvention]
    public class ApiController<TDto> : Controller where TDto : class
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Service = HttpContext.RequestServices.GetService<IService<TDto>>();
            ListService = HttpContext.RequestServices.GetService<IListService>();
        }
        protected IService<TDto> Service { get; set; }
        protected IListService ListService { get; set; }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult Find(string id)
        {
            try
            {
                TDto dto = Service.Find(id);

                if (dto == null)
                    throw new ValidateException("The specified resource was not found");

                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpGet]
        public virtual IActionResult GetAll([FromQuery] ApiParameter parameter)
        {
            try
            {
                var parameters = parameter.ProcessParameters(Request.Query);
                parameters.Filters.Add(DtoMetadata.Instance.Convention.ActiveProperty, true);
                var list = ListService.GetList(Service, parameters);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPost]
        public virtual IActionResult Add([FromBody] TDto dto)
        {
            try
            {
                dto = Service.Add(dto);
                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                Response.ContentType = "text/plain";
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpPut]
        [Route("{id}")]
        public virtual IActionResult Update(string id, [FromBody] TDto dto)
        {
            try
            {
                dto = Service.Update(id, dto);
                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }


        [HttpPatch]
        [Route("{id}")]
        public virtual IActionResult PartialUpdate(string id, [FromBody] object changes)
        {
            try
            {
                var dto = Service.Find(id);
                if (dto == null) throw new ValidateException("Resource not found");
                var patch = changes.ToJsonPatchDocument();
                patch.ApplyTo(dto);
                Service.Update(id, dto);
                return Ok(dto);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual IActionResult Delete(string id)
        {
            try
            {
                var result = Service.Delete(id);
                return Ok(result);
            }
            catch (ValidateException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ex.GetInnerMessages());
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.GetInnerMessages());
            }
        }
    }
}
