using MCQInterviews.Models.Domain;
using MCQInterviews.Models.ViewModels;
using MCQInterviews.Services;
using Microsoft.AspNetCore.Mvc;

namespace MCQInterviews.Controllers
{
    public class VoiceSampleController : Controller
    {
        private readonly IVoiceSampleService _voiceSampleService;
        private readonly IWebHostEnvironment _environment;

        public VoiceSampleController(
            IVoiceSampleService voiceSampleService, 
            IWebHostEnvironment environment)
        {
            _voiceSampleService = voiceSampleService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult UploadVoiceSample()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadVoiceSample(VoiceSampleUploadViewModel model)
        {
            if (ModelState.IsValid && model.VoiceSample != null)
            {
                // Define the folder to store uploads
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Define file path
                var filePath = Path.Combine(uploadsFolder, model.VoiceSample.FileName);

                // Upload file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.VoiceSample.CopyToAsync(fileStream);
                }

                // Create a new VoiceSample object with file details
                var voiceSample = new VoiceSample
                {
                    FileName = model.VoiceSample.FileName,
                    FilePath = filePath
                };

                // Upload voice sample (the service will handle setting the candidate info)
                await _voiceSampleService.UploadVoiceSampleAsync(voiceSample);

                return RedirectToAction("UploadSuccess");
            }

            return View(model);
        }

        // Success view after uploading
        public IActionResult UploadSuccess()
        {
            return View();
        }
    }
}
