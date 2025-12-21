namespace SpaceReserve.AppService.Contracts;

using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.AppService.DTOs.RequestHistory;

public interface IRequestHistoryService
{
    Task<bool> UpdateUserRequestStatusAsync(int requestId, byte statusId, string subjectId);
    Task<IEnumerable<BookingStatusModel>> GetRequestStatusesAsync();
    Task<IEnumerable<RequestHistoryDTO>?> GetAllRequestHistory(int seatId, int pageNo, int pageSize);
    Task<IEnumerable<RequestHistoryDTO>?> GetAllRequestHistoryByStatus(int seatId, int status, int pageNo, int pageSize);
    Task<User?> GetBySubjectIdAsync(string subjectId);
    Task<int> GetUserSeatID(int userId);
}
