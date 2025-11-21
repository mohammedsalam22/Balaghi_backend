using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;

namespace InternetApplications.Abstractions
{
    public interface IFileUploadService
    {
        Task<List<UploadedFileDto>> UploadAsync(List<IFormFile> files, string folder, CancellationToken ct = default);
    }
}
