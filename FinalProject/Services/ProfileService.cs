
namespace FinalProject.Services
{
    public class ProfileService : IProfileService

    {
        private readonly IWebHostEnvironment _environment;

        public ProfileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadProfileImage(IFormFile profileImage, string userId, string existingImageUrl)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName).ToLower();
                if (extension != ".jpg" && extension != ".png")
                {
                    throw new InvalidOperationException("Image must be in png or jpg format.");
                }

                var userDirectory = Path.Combine(_environment.WebRootPath, "images", userId);
                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(userDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                // Optionally delete the old image
                if (!string.IsNullOrEmpty(existingImageUrl))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, existingImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                return $"/images/{userId}/{fileName}";
            }

            throw new InvalidOperationException("Please select an image to upload.");
        }
    }
}
