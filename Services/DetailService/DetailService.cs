using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;
using vue_ts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace vue_ts.Services.DetailService
{
    public class DetailService : IDetailService
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<DetailService> logger;
        private long mobileNumber;

        public DetailService(ApplicationDbContext context, ILogger<DetailService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public BaseResponse CreateDetail(CreateDetailRequest request)
        {
            try
            {
                if (request == null)
                {
                    logger.LogWarning("Received null CreateDetailRequest");
                    return new BaseResponse(false, StatusCodes.Status400BadRequest, new { message = "Invalid request data" });
                }

                // Validate inputs
                var errors = new List<string>();
                if (string.IsNullOrEmpty(request.title)) errors.Add("Title is required");
                if (string.IsNullOrEmpty(request.fullName)) errors.Add("Full name is required");
                

                if (string.IsNullOrEmpty(request.email) || !new EmailAddressAttribute().IsValid(request.email))
                    errors.Add("Valid email is required");
                if (string.IsNullOrEmpty(request.nicNumber)) errors.Add("NIC number is required");
                if (string.IsNullOrEmpty(request.nationality)) errors.Add("Nationality is required");

                if (errors.Any())
                {
                    logger.LogWarning("Validation errors in CreateDetailRequest: {Errors}", string.Join("; ", errors));
                    return new BaseResponse(false, StatusCodes.Status400BadRequest, new { message = string.Join("; ", errors), errors });
                }

                DetailModel newDetail = new DetailModel
                {
                    title = request.title,
                    fullName = request.fullName,
                    mobileNumber = request.mobileNumber,
                    email = request.email,
                    nicNumber = request.nicNumber,
                    nationality = request.nationality
                };

                context.Add(newDetail);
                context.SaveChanges();

                return new BaseResponse(true, StatusCodes.Status200OK, new { message = "Successfully created the new Customer", id = newDetail.id });
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database error while creating Customer");
                return new BaseResponse(false, StatusCodes.Status400BadRequest, new { message = "Invalid data provided: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating Customer");
                return new BaseResponse(false, StatusCodes.Status500InternalServerError, new { message = "Internal server error: " + ex.Message });
            }
        }
    }
}