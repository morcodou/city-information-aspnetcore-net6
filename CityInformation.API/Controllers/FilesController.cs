using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInformation.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public FilesController(FileExtensionContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }


        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var path = "file-download.pdf";
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }


            if(!_contentTypeProvider.TryGetContentType(path, out var contentType)){
                contentType = "application/octet-stream";
            }
            var bytes  = System.IO.File.ReadAllBytes(path);
            return File(bytes, contentType, Path.GetFileName(path));
        }
    }
}
