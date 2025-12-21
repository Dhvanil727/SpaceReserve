using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Admin.AppService.Contracts;

public interface IRequestHistoryService
{
    Task<bool> UpdateUserRequestStatusAsync(int requestId, byte statusId, string subjectId);
    public Task<List<RequestHistoryDto>> GetRequestHistoryAsync(int? sort, int pageNo, int pageSize);
    public Task<List<BookingStatusDto>> GetRequestStatusDropdwon();
    public Task<GetSingleRequestHistoryDto> GetRequestHistoryByIdAsync(int requestId);
}
