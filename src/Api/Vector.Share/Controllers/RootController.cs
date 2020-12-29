using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Vector.Share.Configuration;
using Vector.Share.Data.Models;
using Vector.Share.DTO;
using Vector.Share.Extensions;
using Vector.Share.Services;

namespace Vector.Share.Controllers
{
    [ApiController, Route("/api")]
    public class RootController : ControllerBase
    {
        private readonly IOptions<ServerConfiguration> _configuration;
        private readonly IFileService _fileService;
        private readonly ISchedulerService _schedulerService;

        public RootController(
            IOptions<ServerConfiguration> configuration,
            IFileService fileService,
            ISchedulerService schedulerService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _schedulerService = schedulerService ?? throw new ArgumentNullException(nameof(schedulerService));
        }

        [HttpGet("{identifier}")]
        public async Task<IActionResult> GetAsync([FromRoute] string identifier)
        {
            UploadedFile file;

            try
            {
                file = await _fileService.GetFileAsync(identifier);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }

            byte[] fileData = await  System.IO.File.ReadAllBytesAsync(file.Path);
            return File(fileData, file.ContentType);
        }

        [HttpGet, HttpPost, Route("delete/{identifier}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string identifier)
        {
            try
            {
                await _fileService.DeleteFileAsync(identifier);
                await _schedulerService.DeleteScheduledDeletionAsync(identifier);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("upload"), RequestFormLimits(MultipartBodyLengthLimit = 1073741274), RequestSizeLimit(1073741274)]
        public async Task<IActionResult> UploadAsync([FromForm] UploadModel model)
        {
            byte[] fileData = await model.FileData.GetFileDataAsync();
            UploadedFile file = await _fileService.SaveFileAsync(model.FileData.FileName, fileData, model.Lifetime, model.FileData.ContentType);
            await _schedulerService.ScheduleDeletionAsync(file.Identifier, file.Lifetime);

            var result = new UploadResultModel
            {
                Url = $"{_configuration.Value.ServerName}/{file.Identifier}"
            };

            return Ok(result);
        }
    }
}
