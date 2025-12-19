using AttendEdgeWebService.Infrastructure.CustomException;
using AttendEdgeWebService.Infrastructure.IService;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AttendEdgeWebService.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        #region Constructor
        private readonly IForgotPasswordService _service;

        public ForgotPasswordController(IForgotPasswordService service)
        {
            _service = service;
        }
        #endregion

        [HttpPost]
        public HttpResponseMessage Initiate(Domain.ForgotPassword mForgotPassword)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Initiate(mForgotPassword);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (APIRequestFailedException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
