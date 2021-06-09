using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NextMasjid.Backend.Core.Data;
using NextMasjid.Backend.API.Models;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("contact")]
    public class ContactController : BaseController<ContactController>
    {
        //[HttpPost("submit")]
        //public IActionResult Submit([FromForm] ContactModel contact)
        //{
        //    // todo send email
        //    return CreatedAtAction("", new { });
        //}
    }

    public class ContactModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
    }
}
