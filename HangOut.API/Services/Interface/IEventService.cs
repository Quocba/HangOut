using HangOut.Domain.Paginate;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Event;
using HangOut.Domain.Payload.Response.Event;
using OrbitMap.Domain.Paginate.Interfaces;

namespace HangOut.API.Services.Interface
{
    public interface IEventService
    {
        Task<ApiResponse> CreateEvent(CreateEventRequest request);
        Task<ApiResponse<Paginate<GetEventsResponse>>> GetEvents(int pageNumber, int pageSize, string searchKey, string location, string businessName);
        Task<ApiResponse<GetEventResponse>> GetEvent(Guid eventId);
        Task<ApiResponse<string>> EditEvent(Guid eventId,EditEventRequest request);
        Task<ApiResponse<IPaginate<GetEventsResponse>>> GetEventsByBO(int page, int size);
        Task<ApiResponse<string>>DeleteEvent(Guid eventId);
    }
}
