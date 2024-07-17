using BlogWebApi.Helpers;
using BlogWebApi.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace BlogWebApi.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IOptions<CloudinarySettings> config)
        {
            Account account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(1200).Crop("fill"),
                    Folder = "BlogApp",
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            };
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string imageUrl)
        {
            var regex = new Regex(@"v\d+\/(.+)\.\w+");
            var match = regex.Match(imageUrl);

            var publicId = match.Groups[1].Value;
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);

        }
    }
}

