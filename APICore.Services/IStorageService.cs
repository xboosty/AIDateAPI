using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace APICore.Services
{
    public interface IStorageService
    {
        Task<PutObjectResponse> UploadFile(IFormFile file, string guid, string folderName);

        Task<GetObjectResponse> GetObject(string objectKey, string folderName);

        Task<DeleteObjectResponse> DeleteFile(string key, string folderName);
    }
}