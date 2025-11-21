using InternetApplications.Abstractions;
using Microsoft.AspNetCore.Http;
using Application.DTOs;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InternetApplications.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<List<UploadedFileDto>> UploadAsync(List<IFormFile> files, string folder, CancellationToken ct = default)
        {
            var results = new List<UploadedFileDto>();
            var uploadPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                if (file.Length <= 0) continue;

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var fullPath = Path.Combine(uploadPath, fileName);
                var relativePath = $"/uploads/{folder}/{fileName}";

                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream, ct);

                results.Add(new UploadedFileDto
                {
                    FileName = file.FileName,
                    FilePath = relativePath,
                    ContentType = file.ContentType
                });
            }

            return results;
        }
    }
}
