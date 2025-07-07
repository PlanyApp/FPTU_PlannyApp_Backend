using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanyApp.API.Models;
using PlanyApp.Service.Dto.Invoice;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePendingInvoice([FromBody] RequestCreatePendingInvoice request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.ErrorResponse("Dữ liệu không hợp lệ", ModelState));

            var referenceCode = await _invoiceService.CreatePendingInvoiceAsync(request);

            if (string.IsNullOrEmpty(referenceCode))
                return BadRequest(ApiResponse<string>.ErrorResponse("Không tạo được invoice"));

            return Ok(ApiResponse<object>.SuccessResponse(new { ReferenceCode = referenceCode }, "Tạo hóa đơn tạm thành công"));
        }
        [HttpGet("list-pending-invoices")]
        public async Task<IActionResult> GetPendingInvoices()
        {
            var invoices = await _invoiceService.GetPendingInvoicesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(invoices, "Lấy danh sách hóa đơn tạm thành công"));
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdatePendingInvoice([FromBody] RequestUpdateInvoice request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.ErrorResponse("Dữ liệu không hợp lệ", ModelState));

            try
            {
                var groupId = await _invoiceService.UpdatePendingInvoiceAsync(request);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { groupId },
                    "Cập nhật hóa đơn thành công"
                ));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }
    
    }

}
