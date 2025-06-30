using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Dto.ImageS3;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using PlanyApp.Service.Dto;
using System.Collections.Generic;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Uploads an image to the S3 bucket and creates a corresponding record in the database.
        /// </summary>
        /// <param name="request">The image upload request.</param>
        /// <returns>The result of the upload operation.</returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(ServiceResponseDto<ImageS3Dto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResponseDto<ImageS3Dto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("File is not selected or empty.");
            }

            var result = await _imageService.UploadImageAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves all images associated with a specific reference type and ID.
        /// Supports both regular database images and S3 images based on the s3 query parameter.
        /// </summary>
        /// <param name="referenceType">The type of the reference (e.g., "user", "item", "place", etc.)</param>
        /// <param name="referenceId">The ID of the reference entity</param>
        /// <param name="s3">Optional parameter to specify S3 images (true) or regular database images (false). Default is false.</param>
        /// <returns>A list of images associated with the reference</returns>
        [HttpGet("by-reference/{referenceType}/{referenceId}")]
        [ProducesResponseType(typeof(ServiceResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagesByReference(string referenceType, int referenceId, [FromQuery] bool s3 = false)
        {
            var result = await _imageService.GetImagesByReferenceAsync(referenceType, referenceId, s3);

            if (!result.IsSuccess)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Legacy endpoint: Retrieves S3 images only for backward compatibility.
        /// </summary>
        /// <param name="referenceType">The type of the reference</param>
        /// <param name="referenceId">The ID of the reference entity</param>
        /// <returns>A list of S3 images associated with the reference</returns>
        [HttpGet("s3/by-reference/{referenceType}/{referenceId}")]
        [ProducesResponseType(typeof(ServiceResponseDto<List<ImageS3Dto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponseDto<List<ImageS3Dto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetS3ImagesByReference(string referenceType, int referenceId)
        {
            var result = await _imageService.GetImagesByReferenceAsync(referenceType, referenceId);

            if (!result.IsSuccess)
            {
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
} 