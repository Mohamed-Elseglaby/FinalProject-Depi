namespace FinalProject.Services
{
    public interface IProfileService
    {
         Task<string> UploadProfileImage(IFormFile profileImage, string userId, string existingImageUrl);
    
}
}
