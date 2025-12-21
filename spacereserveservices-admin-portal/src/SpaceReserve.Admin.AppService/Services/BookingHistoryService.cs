using AutoMapper;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.AppService.Enums;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Admin.Utility.Resources;

namespace SpaceReserve.Admin.AppService.Services;

public class BookingHistoryService : IBookingHistoryService
{
    private readonly IBookingHistoryRepository _bookingHistoryRepository;
    private readonly IMapper _mapper;

    public BookingHistoryService(IBookingHistoryRepository bookingHistoryRepository, IMapper mapper)
    {
        _bookingHistoryRepository = bookingHistoryRepository;

        _mapper = mapper;
    }

    public async Task<List<BookingHistoryDto>> GetBookingHistoryAsync(BookingHistoryQueryDto bookingHistoryQueryDto)
    {
        string? nameSearch = bookingHistoryQueryDto.Search;
        int sort = bookingHistoryQueryDto.Sort;

        int pageNumber = bookingHistoryQueryDto.PageNo;
        int pageSize = bookingHistoryQueryDto.PageSize;
        var bookingHistory = await _bookingHistoryRepository.GetAllBookingHistoryAsync(pageNumber, pageSize);


        if (sort == Convert.ToInt32(BookingFilter.Past))
        {
            bookingHistory = await _bookingHistoryRepository.GetPastBookingHistoryAsync(pageNumber, pageSize);
        }
        else if (sort == Convert.ToInt32(BookingFilter.Upcoming))
        {
            bookingHistory = await _bookingHistoryRepository.GetUpcomingBookingHistoryAsync(pageNumber, pageSize);
        }
        if (!string.IsNullOrEmpty(nameSearch))
        {
            bookingHistory = bookingHistory
                .Where(b => (b.User!.FirstName + " " + b.User.LastName)
                .Contains(nameSearch, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        var bookingHistoryDto = _mapper.Map<List<BookingHistoryDto>>(bookingHistory);
        return bookingHistoryDto;
    }

}
