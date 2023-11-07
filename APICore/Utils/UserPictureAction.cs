using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace APICore.Utils
{
    public class UserPictureAction : IMappingAction<User, UserResponse>
    {
        private readonly string s3BaseUrl;

        public UserPictureAction(IConfiguration configuration)
        {
            s3BaseUrl = "https://" + configuration.GetSection("S3")["BucketAidateDocuments"] + ".s3.amazonaws.com/avatars/";
        }

        public void Process(User source, UserResponse destination, ResolutionContext context)
        {
            if (destination.Pictures != null)
            {
                for (int i = 0; i < destination.Pictures.Count; i++)
                {
                    destination.Pictures[i] = s3BaseUrl + destination.Pictures[i];
                }
                destination.Avatar =s3BaseUrl + destination.Avatar;
            }
        }
    }
}
