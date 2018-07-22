using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Hakon.Web.Features.Error
{
    public class ErrorController : Controller
    {
        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
