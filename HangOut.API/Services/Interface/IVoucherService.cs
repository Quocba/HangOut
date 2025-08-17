using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Voucher;
using HangOut.Domain.Payload.Response.Voucher;

namespace HangOut.API.Services.Interface
{
    public interface IVoucherService
    {
        Task<ApiResponse<string>> CreateVoucher(Guid userId,CreateVoucherRequest request);
        Task<ApiResponse<Paginate<GetVouchersResponse>>> GetVoucherByBusiness(Guid businessId, int pageNumber, int pageSize);
        Task<ApiResponse<GetVoucher>> GetVoucher(Guid voucherId);
        Task<ApiResponse<EditVoucherRequest>> EditVoucher(EditVoucherRequest request);
        Task<ApiResponse<string>> ReceiveVoucher(Guid accountId, Guid voucherId);
        Task<ApiResponse<Paginate<GetMyVoucherResponse>>> GetVoucherByAccount(Guid accountId, int pageNumber, int pageSize);
        Task<ApiResponse<string>> DeleteVoucher(Guid voucherId);
        Task<ApiResponse<Paginate<GetVouchersResponse>>>GetVouchersByBusinessOwner(Guid accountId, int pageNumber, int pageSize);
        Task<ApiResponse<Paginate<GetUserVoucherByBusiness>>> GetUserVoucherByBusiness(int pageNumber, int pageSize,Guid userBusinessId,string? email);
        Task<ApiResponse<string>> CLickIsUsed(Guid accountId, Guid voucherId);
    }
}
