using PwCTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PwCTools.Controllers
{
    public class BoardWebApiController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var repo = new BoardRepository();
            var response = Request.CreateResponse();

            response.Content = new StringContent(JsonConvert.SerializeObject(repo.GetColumns()));
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [HttpGet]
        public HttpResponseMessage CanMove(int sourceColId, int targetColId)
        {
            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(JsonConvert.SerializeObject(new { canMove = false }));

            if ((targetColId - sourceColId == 1) || (targetColId - sourceColId == -1))
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(new { canMove = true }));
            }

            return response;
        }

        [Route("api/BoardWebApi/MoveTask")]
        [HttpPost]
        public HttpResponseMessage MoveTask(JObject moveTaskParams)
        {
            dynamic json = moveTaskParams;
            var repo = new BoardRepository();
            repo.MoveTask((int)json.taskId, (int)json.targetColId);

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [Route("api/BoardWebApi/EditTask")]
        [HttpPost]
        public HttpResponseMessage EditTask(JObject editTaskParams)
        {
            dynamic json = editTaskParams;
            var repo = new BoardRepository();
            repo.EditTask((int)json.taskId, (string)json.taskName, (string)json.taskDescription);

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}