using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using PlanyApp.Repository.Models;
using System.Linq;
using System.Collections.Generic;

namespace PlanyApp.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;

        public ImageService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IAmazonS3 s3Client)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _s3Client = s3Client;
        }

        public async Task<ServiceResponseDto<ImageS3Dto>> UploadImageAsync(UploadImageRequestDto request)
        {
            var (isValid, errorMessage) = await ValidateReferenceAsync(request.ReferenceType, request.ReferenceId);
            if (!isValid)
            {
                return new ServiceResponseDto<ImageS3Dto> { IsSuccess = false, Message = errorMessage };
            }

            var response = new ServiceResponseDto<ImageS3Dto>();
            string? tempFilePath = null;
            try
            {
                var s3Settings = _configuration.GetSection("S3Settings");
                var bucketName = s3Settings["BucketName"];
                var serviceURL = s3Settings["ServiceURL"];

                var key = $"{Guid.NewGuid()}-{request.File.FileName}";

                // Save to a temporary file to ensure reliable uploads with this S3 provider
                tempFilePath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = tempFilePath, // Use the file path instead of a stream
                    ContentType = request.File.ContentType,
                    CannedACL = S3CannedACL.PublicRead // Make the object publicly readable
                };

                await _s3Client.PutObjectAsync(putRequest);

                var imageUrl = $"{serviceURL}/{bucketName}/{key}";

                var imageS3 = new ImageS3
                {
                    ReferenceType = request.ReferenceType,
                    ReferenceId = request.ReferenceId,
                    ImageUrl = imageUrl,
                    ContentType = request.File.ContentType,
                    FileSizeKb = (int)(request.File.Length / 1024),
                    IsPrimary = request.IsPrimary.HasValue ? request.IsPrimary.Value : null,
                    Caption = request.Caption,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ImageS3Repository.AddAsync(imageS3);
                await _unitOfWork.SaveAsync();

                response.Data = _mapper.Map<ImageS3Dto>(imageS3);
                response.IsSuccess = true;
                response.Message = "Image uploaded successfully.";
            }
            catch (AmazonS3Exception s3Ex)
            {
                // More specific S3 exception handling to get detailed error codes
                var statusCode = s3Ex.StatusCode.ToString();
                var errorCode = s3Ex.ErrorCode ?? "N/A";
                var errorDetails = $"S3 service returned an error. Status Code: {statusCode}. Error Code: {errorCode}. Message: {s3Ex.Message}";
                Console.WriteLine($"[ImageService] S3 Exception: {errorDetails}. Full Exception: {s3Ex.ToString()}");
                
                response.IsSuccess = false;
                response.Message = "An error occurred during the file upload process.";
                response.Errors.Add(errorDetails);
            }
            catch (Exception ex)
            {
                // Log the full exception to the console for detailed diagnostics
                Console.WriteLine($"[ImageService] Exception during image upload: {ex.ToString()}");

                response.IsSuccess = false;
                response.Message = "An unexpected error occurred while saving the image data. See errors for details.";
                
                // Recursively add all inner exception messages to the error list for the client
                var innerEx = ex;
                while (innerEx != null)
                {
                    response.Errors.Add(innerEx.Message);
                    innerEx = innerEx.InnerException;
                }
            }
            finally
            {
                // Ensure the temporary file is always deleted
                if (tempFilePath != null && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
            return response;
        }

        private async Task<(bool IsValid, string ErrorMessage)> ValidateReferenceAsync(string referenceType, int referenceId)
        {
            bool exists;
            switch (referenceType.ToLower())
            {
                case "user":
                    exists = await _unitOfWork.UserRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "item":
                    exists = await _unitOfWork.ItemRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "place":
                    exists = await _unitOfWork.PlaceRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "hotel":
                    exists = await _unitOfWork.HotelRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "transportation":
                    exists = await _unitOfWork.TransportationRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "plan":
                    exists = await _unitOfWork.PlanRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "challenge":
                    exists = await _unitOfWork.ChallengeRepository.GetByIdAsync(referenceId) != null;
                    break;
                case "userchallengeprogress":
                    exists = await _unitOfWork.UserChallengeProgressRepository.GetByIdAsync(referenceId) != null;
                    break;
                default:
                    return (false, $"Unsupported or invalid ReferenceType: {referenceType}");
            }

            if (!exists)
            {
                return (false, $"Reference entity with ID '{referenceId}' for type '{referenceType}' not found.");
            }

            return (true, string.Empty);
        }

        public async Task<ServiceResponseDto<object>> GetImagesByReferenceAsync(string referenceType, int referenceId, bool isS3)
        {
            var response = new ServiceResponseDto<object>();

            try
            {
                var (isValid, errorMessage) = await ValidateReferenceAsync(referenceType, referenceId);
                if (!isValid)
                {
                    return new ServiceResponseDto<object> 
                    { 
                        IsSuccess = false, 
                        Message = errorMessage 
                    };
                }

                if (isS3)
                {
                    var s3Images = await _unitOfWork.ImageS3Repository.FindAsync(
                        img => img.ReferenceType.ToLower() == referenceType.ToLower() && 
                              img.ReferenceId == referenceId);

                    response.Data = _mapper.Map<List<ImageS3Dto>>(s3Images);
                    response.IsSuccess = true;
                    response.Message = s3Images.Any() 
                        ? $"Successfully retrieved {s3Images.Count} S3 images."
                        : "No S3 images found for the specified reference.";
                }
                else
                {
                    var regularImages = await _unitOfWork.ImageRepository.FindAsync(
                        img => img.ReferenceType.ToLower() == referenceType.ToLower() && 
                              img.ReferenceId == referenceId);

                    response.Data = _mapper.Map<List<ImageDto>>(regularImages);
                    response.IsSuccess = true;
                    response.Message = regularImages.Any() 
                        ? $"Successfully retrieved {regularImages.Count} regular images."
                        : "No regular images found for the specified reference.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while retrieving the images.";
                response.Errors.Add(ex.Message);
                
                // Log the error
                Console.WriteLine($"[ImageService] Exception during image retrieval: {ex}");
            }

            return response;
        }

        public async Task<ServiceResponseDto<List<ImageS3Dto>>> GetImagesByReferenceAsync(string referenceType, int referenceId)
        {
            var response = new ServiceResponseDto<List<ImageS3Dto>>();

            try
            {
                var (isValid, errorMessage) = await ValidateReferenceAsync(referenceType, referenceId);
                if (!isValid)
                {
                    return new ServiceResponseDto<List<ImageS3Dto>> 
                    { 
                        IsSuccess = false, 
                        Message = errorMessage 
                    };
                }

                var images = await _unitOfWork.ImageS3Repository.FindAsync(
                    img => img.ReferenceType.ToLower() == referenceType.ToLower() && 
                          img.ReferenceId == referenceId);

                response.Data = _mapper.Map<List<ImageS3Dto>>(images);
                response.IsSuccess = true;
                response.Message = images.Any() 
                    ? $"Successfully retrieved {images.Count} images."
                    : "No images found for the specified reference.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred while retrieving the images.";
                response.Errors.Add(ex.Message);
                
                // Log the error
                Console.WriteLine($"[ImageService] Exception during image retrieval: {ex}");
            }

            return response;
        }
    }
} 