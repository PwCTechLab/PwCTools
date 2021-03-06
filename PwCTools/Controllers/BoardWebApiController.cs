﻿using PwCTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace PwCTools.Controllers
{
    public class BoardWebApiController : ApiController
    {
        [HttpGet, ActionName("Get")]
        public HttpResponseMessage Get(int projectId)
        {
            //Set Default Project
            System.Web.HttpContext.Current.Cache["ActiveProject"] = projectId;

            using (var repo = new BoardRepository())
            {
                var response = Request.CreateResponse();

                response.Content = new StringContent(JsonConvert.SerializeObject(repo.GetColumns()));
                response.StatusCode = HttpStatusCode.OK;

                return response; 
            }
        }

        [HttpGet, ActionName("CanMove")]
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

        [HttpGet, ActionName("GetComments")]
        public HttpResponseMessage GetComments(int taskId)
        {
            using (var repo = new BoardRepository())
            {
                var response = Request.CreateResponse();

                response.Content = new StringContent(JsonConvert.SerializeObject(repo.GetComments(taskId)));
                response.StatusCode = HttpStatusCode.OK;

                return response; 
            }
        }

        [Route("api/BoardWebApi/MoveTask")]
        [HttpPost]
        public HttpResponseMessage MoveTask(JObject moveTaskParams)
        {
            dynamic json = moveTaskParams;
            using (var repo = new BoardRepository())
            {
                repo.MoveTask((int)json.taskId, (int)json.targetColId);    
            }

            var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.OK;

                return response; 
        }

        [Route("api/BoardWebApi/EditTask")]
        [HttpPost]
        public HttpResponseMessage EditTask(JObject editTaskParams)
        {
            dynamic json = editTaskParams;
            using (var repo = new BoardRepository())
            {
                if (json.taskId == "")
                    repo.AddTask((string)json.taskName, (string)json.taskAssignee, (string)json.taskDueDate, (string)json.taskDescription);
                else
                    repo.EditTask((int)json.taskId, (string)json.taskName, (string)json.taskAssignee, (string)json.taskDueDate, (string)json.taskDescription);    
            }

            var response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.OK;

                return response; 
        }

        [Route("api/BoardWebApi/ArchiveTask")]
        [HttpPost]
        public HttpResponseMessage ArchiveTask(JObject archiveTaskParams)
        {
            dynamic json = archiveTaskParams;
            using (var repo = new BoardRepository())
            {
                repo.ArchiveTask((int)json.taskId); 
            }

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [Route("api/BoardWebApi/DeleteTask")]
        [HttpPost]
        public HttpResponseMessage DeleteTask(JObject deleteTaskParams)
        {
            dynamic json = deleteTaskParams;
            using (var repo = new BoardRepository())
            {
                repo.DeleteTask((int)json.taskId); 
            }

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [Route("api/BoardWebApi/AddComment")]
        [HttpPost]
        public HttpResponseMessage AddComment(JObject addCommentParams)
        {
            dynamic json = addCommentParams;
            using (var repo = new BoardRepository())
            {
                repo.AddComment((int)json.taskId, (string)json.comment, User.Identity.GetUserId(), ((string)json.commentId == "") ? (int?)null : (int?)json.commentId);
            }

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [HttpGet, ActionName("GetProjectUsers")]
        public HttpResponseMessage GetProjectUsers()
        {
            using (var repo = new BoardRepository())
            {
                var response = Request.CreateResponse();

                response.Content = new StringContent(JsonConvert.SerializeObject(repo.GetProjectUsers()));
                response.StatusCode = HttpStatusCode.OK;

                return response;
            }
        }

    }
}