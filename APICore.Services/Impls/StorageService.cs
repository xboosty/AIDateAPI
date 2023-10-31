using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using APICore.Services.Exceptions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class StorageService : IStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<IStorageService> _localizer;
        private readonly IAmazonS3 _amazons3;

        public StorageService(IConfiguration configuration, IStringLocalizer<IStorageService> localizer)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _amazons3 = new AmazonS3Client(_configuration.GetSection("S3")["AccessKey"], _configuration.GetSection("S3")["SecretKey"], new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_configuration.GetSection("S3")["BucketRegionName"])
            });
        }

        public async Task<PutObjectResponse> UploadFile(IFormFile file, string guid, string folderName)
        {
            var bucketName = _configuration.GetSection("S3")["BucketAidateDocuments"] + $"/{folderName}";
            var putRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = guid,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
            };

            var result = await _amazons3.PutObjectAsync(putRequest);
            var successful = result.HttpStatusCode == System.Net.HttpStatusCode.OK;
            if (!successful)
            {
                throw new BaseBadRequestException();
            }

            return result;
        }

        public async Task<GetObjectResponse> GetObject(string objectKey, string folderName)
        {
            var req = new GetObjectRequest()
            {
                BucketName = _configuration.GetSection("S3")["BucketAutomedicDocuments"] + $"/{folderName}",
                Key = objectKey
            };

            var response = await _amazons3.GetObjectAsync(req);
            return response;
        }

        public async Task<DeleteObjectResponse> DeleteFile(string objectKey, string folderName)
        {
            var req = new DeleteObjectRequest()
            {
                BucketName = _configuration.GetSection("S3")["BucketAutomedicDocuments"] + $"/{folderName}",
                Key = objectKey
            };
            var response = await _amazons3.DeleteObjectAsync(req);
            return response;
        }
    }
}