using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Payload.Request.Voucher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers
{
    [ApiController]
    [Route("api/v1/voucher")]
    public class VoucherController(ILogger _logger, IVoucherService _voucherService) : Controller
    {
        [HttpPost("create-voucher")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = UserUtil.GetAccountId(HttpContext);
                var response = await _voucherService.CreateVoucher(userId!.Value,request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _logger.Error("[Create Voucher API] " + ex.Message,ex.StackTrace);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("get-voucher-by-business")]
        public async Task<IActionResult> GetVoucherByBusiness([FromQuery]Guid businessId, int pageNumber, int pageSize)
        {
            var response = await _voucherService.GetVoucherByBusiness(businessId, pageNumber,pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpGet("get-voucher")]
        public async Task<IActionResult> GetVoucher([FromQuery]Guid voucherId)
        {
            var response = await _voucherService.GetVoucher(voucherId);
            return StatusCode(response.Status,response);
        }

        [HttpPost("receive-voucher")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReceiveVoucher([FromBody] ReceiveVoucher request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _voucherService.ReceiveVoucher(accountId!.Value,request.VoucherId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex) { 
                
                _logger.Error("[Receive Voucher API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-account-voucher")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAccountVoucher([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _voucherService.GetVoucherByAccount(accountId!.Value, pageNumber,pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpPatch("edit-voucher/{voucherId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> EditVoucher(Guid voucherId, [FromBody] EditVoucherRequest request)
        {
            try
            {
                request.VoucherId = voucherId;
                var response = await _voucherService.EditVoucher(request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex) { 
            
                _logger.Error("[Edit Voucher API ]" + ex.Message, ex.StackTrace);
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpDelete("delete-voucher/{voucherId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> DeleteVoucher(Guid voucherId)
        {
            try
            {
                var response = await _voucherService.DeleteVoucher(voucherId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _logger.Error("[Delete Voucher API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-voucher-by-businees-owner")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetVoucherByBusinessOwner([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);

            var response = await _voucherService.GetVouchersByBusinessOwner(accountId!.Value, pageNumber, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpGet("get-user-voucher-by-business")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetUserVoucherByBusiness([FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string? email)
        {
            var userBusinessId = UserUtil.GetAccountId(HttpContext);
            var response = await _voucherService.GetUserVoucherByBusiness(pageNumber, pageSize,userBusinessId!.Value,email);
            return StatusCode(response.Status, response);
        }

        [HttpPut("click-use-voucher")]
        [Authorize(Roles = "BusinessOwner,User")]
        public async Task<IActionResult> ClickUseVoucher([FromBody]ClickUseVoucherRequest request)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                request.AccountId = accountId!.Value;
                var response = await _voucherService.CLickIsUsed(request.AccountId, request.VoucherId);
                return StatusCode(response.Status, response);

            } catch (Exception ex)
            {
                _logger.Error("[Click use voucher request]" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
