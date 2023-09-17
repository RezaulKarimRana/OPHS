﻿using System.Threading.Tasks;
using AMS.Models.ServiceModels.Admin;

namespace AMS.Services.Admin.Contracts
{
    public interface IAdminService
    {
        Task<CheckIfCanCreateAdminUserResponse> CheckIfCanCreateAdminUser();

        Task<CreateAdminUserResponse> CreateAdminUser(CreateAdminUserRequest request);
    }
}
