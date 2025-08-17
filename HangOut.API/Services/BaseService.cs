using HangOut.Domain.Persistence;
using HangOut.Repository.Interface;

namespace HangOut.API.Services;

public abstract class BaseService<T> where T : class
{
    protected IUnitOfWork<HangOutContext> _unitOfWork;
    protected ILogger _logger;
    protected IHttpContextAccessor _httpContextAccessor;

    public BaseService(IUnitOfWork<HangOutContext> unitOfWork, ILogger logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
}