using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebJob.Models;
using Microsoft.AspNet.Identity;


namespace WebJob.Controllers.API
{
    public class JobsController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Save(following_job follow)
        {
            
            var userID = User.Identity.GetUserId();
            follow.id_employee = userID;
            if (userID == null)
                return BadRequest("Please login!");
            JobContextDataContext dt = new JobContextDataContext();

            var following = dt.following_jobs.FirstOrDefault(p => p.id_employee == follow.id_employee && p.id_job_detail == follow.id_job_detail);
            if (following == null)
            {
                dt.following_jobs.InsertOnSubmit(follow);
            }
            else
            {
                dt.following_jobs.DeleteOnSubmit(following);
            }
            dt.SubmitChanges();
            return Ok();
        }
    }
}
