using AttendEdgeWebService.Infrastructure.CustomException;
using AttendEdgeWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AttendEdgeWebService.Controllers
{
    [RoutePrefix("api/UserDocument")]
    public class UserDocumentController : ApiController
    {
        private readonly IUserDocumentService _service;
        public UserDocumentController(IUserDocumentService service)
        {
            _service = service;
        }

        [Authorize, HttpPost]
        public HttpResponseMessage Create(Domain.UserDocument mDocument)
        {
            HttpResponseMessage response;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Create(mDocument));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Authorize, HttpGet, Route("UserId/{userId:int}")]
        public HttpResponseMessage Get(int userId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Get(userId));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Authorize, HttpDelete, Route("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Delete(id);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
