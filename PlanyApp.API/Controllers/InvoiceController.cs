using Microsoft.AspNetCore.Mvc;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Interfaces;

namespace PlanyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                return BadRequest(ModelState);

            try
            {
                var referenceCode = await _invoiceService.CreatePendingInvoiceAsync(request);
                return Ok(new { ReferenceCode = referenceCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("list-pending-invoices")]
        public async Task<IActionResult> GetPendingInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetPendingInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPut("")]
        public async Task<IActionResult> UpdatePendingInvoice([FromBody] RequestUpdateInvoice request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _invoiceService.UpdatePendingInvoiceAsync(request);
                if (result)
                    return Ok(new { Message = "Invoice updated successfully" });
                else
                    return NotFound(new { Message = "Invoice not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
