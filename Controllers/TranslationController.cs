using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly IUowTranslation _repository;
        private readonly TranslationService _translationService;
        public TranslationController(IUowTranslation repository, TranslationService translationService)
        {
            _repository = repository;
            _translationService = translationService;
        }

        /// <summary>
        /// Get translation for a given key from Global or Local scope
        /// </summary>
        [HttpGet("get-Globaltranslation")]
        public IActionResult GetTranslation([FromQuery] string key, [FromQuery] string scope = "Global")
        {
            

            string translatedText = _translationService.GetTranslation(key, scope);
            return Ok(new { key, translation = translatedText, scope });
        }

        [HttpGet("get-translation")]
        public IActionResult GetLocalTranslation([FromQuery] string key, [FromQuery] string scope = "")
        {
            // Call a local translation using a specific controller name

            string translatedText = _translationService.GetTranslation(key, scope);
            return Ok(new { key, translation = translatedText, scope });
        }

        /// <summary>
        /// Set the application's language (Global)
        /// </summary>
        [HttpPost("set-language/{languageCode}")]
        public IActionResult SetLanguage(string languageCode)
        {
            _translationService.SetLanguage(languageCode);
            return Ok(new { message = $"Language set to {languageCode}" });
        }



        /// <summary>
        /// Generate resource files dynamically from database
        /// </summary>
        [HttpPost("generate-resources")]
        public async Task<IActionResult> GenerateResourceFiles()
        {
            await _translationService.GenerateResourceFiles(_repository);
            return Ok(new { message = "Resource files generated successfully!" });
        }

        [HttpPost("Export-resources")]
        public async Task<IActionResult> ExportResourceFiles(string? ResourceName, string? Culture)
        {
            var list = await _repository.TranslationDALRepo.ExportResourceFiles(ResourceName, Culture);

            if (list != null && list.Any())  // Ensure there are translations returned
                return Ok(list);

            return BadRequest("Failed to export translation.");
        }




        [HttpPost("InsertTranslationList")]
        public async Task<IActionResult> InsertTranslationList([FromBody] List<Translation> lstmodel)
        {
            bool success = await _repository.TranslationDALRepo.InsertOrUpdateTranslationList(lstmodel);
            if (success)
                return Ok("Translation added successfully.");

            return BadRequest("Failed to insert translation.");
        }
    }
}
