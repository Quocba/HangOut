using HangOut.API.Common.Utils;
using HangOut.API.Services.Interface;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Business;
using HangOut.Domain.Payload.Response.Business;
using HangOut.Domain.Payload.Response.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitMap.Domain.Paginate.Interfaces;
using Org.BouncyCastle.Tsp;

namespace HangOut.API.Controllers
{
    [ApiController]
    [Route("api/v1/business")]
    public class BusinessController(ILogger _looger, IBusinessService _businessService, IReviewService _reviewService)
        : Controller
    {
        [HttpPost("create-business-owner")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBusiness([FromForm] CreateBusinessOwnerRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _businessService.CreateBusinessOwner(request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _looger.Error("[Create Business API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("register-business-account")]
        public async Task<IActionResult> RegisterBusinessOwner([FromForm] RegisterBusinessRequest request)
        {
            try
            {
                var response = await _businessService.RegisterBusinessOwner(request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _looger.Error("[Register Business Owner]" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPatch("edit-business/{businessId}")]
        public async Task<IActionResult> EditBusiness(Guid businessId, [FromForm] EditBusinessRequest request)
        {
            try
            {
                var response = await _businessService.EditBusiness(businessId, request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                _looger.Error("[Edit Business API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-business")]
        public async Task<IActionResult> GetBusiness([FromQuery] int pageNumber, [FromQuery] int pageSize,
            [FromQuery] string? category,
            [FromQuery] string? province, [FromQuery] string? businessName)
        {
            Guid? accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _businessService.GetAllBusinessResponse(accountId, pageNumber, pageSize, category,
                province, businessName);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("delete-business/{businessId}")]
        [Authorize(Roles = "BusinessOwner, Admin")]
        public async Task<IActionResult> DeleteBusiness(Guid businessId)
        {
            try
            {
                var response = await _businessService.DeleteBusiness(businessId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                _looger.Error("[Delete Business API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-business-detail")]
        public async Task<IActionResult> GetBusinessDetail([FromQuery] Guid businessId)
        {
            var response = await _businessService.GetBusinessDetail(businessId);
            return StatusCode(response.Status, response);
        }

        [HttpPut("active-business/{businessId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActiveBusiness(Guid businessId)
        {
            try
            {
                var response = await _businessService.ActiveBusiness(businessId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                _looger.Error("[Active Business API]" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("{id}/reviews")]
        [ProducesResponseType(typeof(ApiResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsByBusinessId([FromRoute] Guid id, [FromQuery] int page = 1,
            [FromQuery] int size = 30,
            [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
        {
            try
            {
                var accountId = UserUtil.GetAccountId(HttpContext);
                var response = await _reviewService.GetReviewsByBusinessIdAsync(id, page, size, sortBy, isAsc);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _looger.Error("[Get Reviews By Business Id API] " + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-account-by-business")]
        public async Task<IActionResult> GetBusinessAccount([FromQuery] Guid businessId)
        {
                var response = await _businessService.GetBusinessAccount(businessId);
                return StatusCode(response.Status, response);
        }

        [HttpGet("get-business-by-owner")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> GetBusinessByOwner([FromQuery]int pageNumber, [FromQuery]int pageSize)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _businessService.GetBusinessByOwner(accountId!.Value, pageNumber, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpPost("like-dislike-business")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> LikeOrDisLikeBusiness([FromBody]LikeBusinessRequest request)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _businessService.FavoriteBusiness(accountId!.Value,request.BusinessId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("get-favorite-business")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetFavoriteBusiness([FromQuery]int pageNumber, [FromQuery]int pageSize, [FromQuery]string? categoryName)
        {
            var accountId = UserUtil.GetAccountId(HttpContext);
            var response = await _businessService.GetBusinessFavorite(accountId!.Value, pageNumber, pageSize,categoryName);
            return StatusCode(response.Status, response);
        }

        [HttpPost("create-business")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateBussiness([FromForm]CreateReviewBusiness request)
        {
            try
            {
                var response = await _businessService.CreateBusiness(request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                _looger.Error("[Create Business API]" + ex.Message, ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }

        }

    }
}
