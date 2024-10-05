using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Net;
using System.Net.Mail;
using TCViettelFC_API.Repositories.Interfaces;
using static TCViettelFC_API.Repositories.Implementations.CloudinarySettings;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class CloudinarySettings : ICloudinarySetting
    {
       
            public string CloudName { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
            public CloudinarySettings() { }

            // Lấy thông tin settings của Cloudinary từ appsettings.json
            // Người viết: TuanNQ
            // Ngày: 03/10/2024
            public CloudinarySettings GetCloudinarySettings()
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var cloudinarySettings = new CloudinarySettings();
                configuration.GetSection("CloudinarySettings").Bind(cloudinarySettings);
                return cloudinarySettings;
            }
        // Upload ảnh lên Cloudinary
        // Người viết: TuanNQ
        // Ngày: 03/10/2024
        public ImageUploadResult CloudinaryUpload(IFormFile f)
            {
                CloudinarySettings cs = GetCloudinarySettings();
                Account account = new Account(cs.CloudName, cs.ApiKey, cs.ApiSecret);
                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(f.FileName, f.OpenReadStream())

                };
                return cloudinary.Upload(uploadParams);
            }
    }
}
