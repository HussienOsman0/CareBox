using CareBox.BLL.DTOs.InvoiceDto;
using CareBox.BLL.Services.InvoiceManagementService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceManagementService _invoiceManagementService;

        public InvoicesController(IInvoiceManagementService invoiceManagementService)
        {
            _invoiceManagementService = invoiceManagementService;
        }

        #region Helper
        private int GetCurrentUserId()
        {
            var userIdCliam = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdCliam == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdCliam.Value);
        }
        #endregion

        #region Add Custom Items To Invoice
        [HttpPut("AddCustomItemsToInvoice")]
        [Authorize(Roles = "SERVICEPROVIDER")] // (اختياري) لو عندك Role معين لمقدم الخدمة
        public async Task<IActionResult> AddCustomItemsToInvoice([FromBody] AddMultipleInvoiceItemsDto model)
        {
            try
            {
                var providerUserId = GetCurrentUserId();

                var result = await _invoiceManagementService.AddCustomItemsToInvoiceAsync(providerUserId, model);

                if (result)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Items have been successfully added to the draft invoice."
                    });
                }
                return BadRequest(new { Success = false, Message = "Failed to add items to the invoice." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });

            }
        }
        #endregion



        [HttpGet("my-invoices")]
        public async Task<IActionResult> GetMyInvoices()
        {
            try
            {
                var userId = GetCurrentUserId();
                // يمكنك التحقق هنا من الـ Role لتحديد أي دالة يتم استدعاؤها
                if (User.IsInRole("SERVICEPROVIDER"))
                {
                    var providerInvoices = await _invoiceManagementService.GetProviderInvoicesAsync(userId);
                    return Ok(new
                    {
                        success = true,
                        data = providerInvoices
                    });
                }
                else
                {
                    var clientInvoices = await _invoiceManagementService.GetClientInvoicesAsync(userId);
                    return Ok(new
                    {
                        success = true,
                        data = clientInvoices
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
