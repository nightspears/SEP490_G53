using CloudinaryDotNet.Actions;
using TCViettelFC_API.Repositories.Implementations;
using static TCViettelFC_API.Repositories.Implementations.CloudinarySettings;

namespace TCViettelFC_API.Repositories.Interfaces
{
    public interface ICloudinarySetting
    {
        CloudinarySettings GetCloudinarySettings();
        ImageUploadResult CloudinaryUpload(IFormFile f);
    }
}
