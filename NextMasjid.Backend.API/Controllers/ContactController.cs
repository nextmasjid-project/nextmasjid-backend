using Microsoft.AspNetCore.Mvc;

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
