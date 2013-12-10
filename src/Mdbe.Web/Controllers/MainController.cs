using Mdbe.Core.Blog;
using Mdbe.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mdbe.Web.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /[slug]
        public ActionResult Index(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return View("~/Views/Main/Index.cshtml", new IndexModel());

            return ViewPost(slug);
        }

        private ActionResult ViewPost(string slug)
        {
            var model = new ViewPostModel(slug);
            if (model.Post != null)
            {
                return View("~/Views/Main/ViewPost.cshtml", model);
            }
            else
            {
                return HttpNotFound();
            }
        }

    }
}
