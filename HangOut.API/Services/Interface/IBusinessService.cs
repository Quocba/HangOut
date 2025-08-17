using HangOut.Domain.Entities;
using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Business;
using HangOut.Domain.Payload.Response.Business;
using Supabase.Functions.Responses;

namespace HangOut.API.Services.Interface
{
    public interface IBusinessService
    {
        Task<ApiResponse<string>> CreateBusinessOwner(CreateBusinessOwnerRequest request);
        Task<ApiResponse<string>> RegisterBusinessOwner(RegisterBusinessRequest request);
        Task<ApiResponse<string>> EditBusiness(Guid businessId, EditBusinessRequest request);
        Task<ApiResponse<Paginate<Domain.Payload.Response.Business.BusinessListWithHotResponse>>> GetAllBusinessResponse(Guid? accountId, int pageNumber, int pageSize, string? category,string? province, string? businessName);
        Task<ApiResponse<string>> DeleteBusiness(Guid businessId);
        Task<ApiResponse<GetBusinessDetailResponse>> GetBusinessDetail(Guid businessId);
        Task<ApiResponse<string>> ActiveBusiness(Guid businessId);
        Task<ApiResponse<GetBusinessAccount>> GetBusinessAccount(Guid businessIds);
        Task<ApiResponse<Paginate<GetAllBusinessResponse>>> GetBusinessByOwner(Guid accountId,int pageNumber, int pageSize);
        Task<ApiResponse<Paginate<GetAllBusinessResponse>>> GetBusinessFavorite(Guid accountId, int pageNumber, int pageSize, string? categoryName);
        Task<ApiResponse<string>> FavoriteBusiness(Guid accountId,Guid businessId);
        Task<ApiResponse<string>>CreateBusiness(CreateReviewBusiness request);

    }
}
