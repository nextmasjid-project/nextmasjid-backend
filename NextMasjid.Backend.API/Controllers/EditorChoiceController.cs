using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NextMasjid.Backend.Core;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("editorChoice")]
    public class EditorChoiceController : BaseController<EditorChoiceController>
    {
       
        [HttpGet("{lang}")]
        public IEnumerable<EditorChoiceModel> Get(string lang = "ar")
        {
            return Choices.Select(c => new EditorChoiceModel() { Lat = c.Location.Lat, Lng = c.Location.Lng, Notes = c.Notes });
        }
    }
}
