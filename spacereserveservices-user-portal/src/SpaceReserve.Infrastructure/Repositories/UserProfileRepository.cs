using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Data;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly AppDbContext _context;
    public UserProfileRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserProfileBySubjectIdAsync(string subjectId)
    {
        return await _context.Users
                            .AsNoTracking()
                            .AsSplitQuery()
                            .Include(u => u.ModeOfWorkModel)
                            .Include(u => u.DesignationModel)
                            .Include(u => u.CityModel)
                            .Include(u => u.UserWorkingDays)
                                .ThenInclude(uwd => uwd.WorkingDay)
                            .Where(u => u.SubjectId == subjectId && u.DeletedDate == null)
                            .FirstOrDefaultAsync();

    }
    public async Task<SeatConfiguration?> GetSeatConfigurationByUserIdAsync(int userId)
    {
        return await _context.SeatConfigurations.FirstOrDefaultAsync(sc => sc.UserId == userId && sc.DeletedDate == null);
    }

    public async Task<Seat?> GetSeatByIdAsync(short seatId)
    {
        return await _context.Seats.FirstOrDefaultAsync(s => s.SeatId == seatId);
    }

    public async Task<ColumnModel?> GetColumnByIdAsync(short columnId)
    {
        return await _context.ColumnModels.FirstOrDefaultAsync(c => c.ColumnId == columnId);
    }

    public async Task<FloorModel?> GetFloorByIdAsync(short floorId)
    {
        return await _context.FloorModels.FirstOrDefaultAsync(c => c.FloorId == floorId);
    }

    public async Task<List<short>> GetAllAssignedSeatIdsAsync()
    {   
        return await _context.SeatConfigurations
                .AsNoTracking()
                .Select(sc => sc.SeatId)
                .ToListAsync();
    }
    
    public async Task<bool> UpdateUserProfileBySubjectIdAsync(string subjectId, User user, short? seatId)
    {

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.SubjectId == subjectId && u.DeletedDate == null);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.ProfileImageName = user.ProfileImageName;
            existingUser.ProfileImage = user.ProfileImage;
            existingUser.ModeOfWorkId = user.ModeOfWorkId;
            existingUser.DesignationId = user.DesignationId;
            existingUser.CityId = user.CityId;
            existingUser.ModifiedBy = existingUser.UserId;
            existingUser.ModifiedDate = DateTime.Now;

            var userWorkingDays = _context.UserWorkingDays.Where(uw => uw.UserId == existingUser.UserId && uw.DeletedDate == null).ToList();

            foreach (var userWorkingDay in userWorkingDays)
            {
                userWorkingDay.DeletedBy = existingUser.UserId;
                userWorkingDay.DeletedDate = DateTime.Now;
                _context.UserWorkingDays.Update(userWorkingDay);
            }

            foreach (var userWorkingDay in user.UserWorkingDays)
            {
                userWorkingDay.UserId = existingUser.UserId;
                userWorkingDay.CreatedBy = existingUser.UserId;
                userWorkingDay.CreatedDate = DateTime.Now;
                _context.UserWorkingDays.Add(userWorkingDay);
            }

            var seatConfiguration = await _context.SeatConfigurations.FirstOrDefaultAsync(s => s.UserId == existingUser.UserId && s.DeletedDate == null);
            
            if (seatId != null)
            {
                if (seatConfiguration != null)
                {
                    seatConfiguration.UserId = existingUser.UserId;
                    seatConfiguration.SeatId = (short)seatId;
                    seatConfiguration.ModifiedBy = existingUser.UserId;
                    seatConfiguration.ModifiedDate = DateTime.Now;
                    _context.SeatConfigurations.Update(seatConfiguration);
                }
                else
                {
                    var newSeatConfiguration = new SeatConfiguration
                    {
                        UserId = existingUser.UserId,
                        SeatId = (short)seatId,
                        CreatedBy = existingUser.UserId,
                        CreatedDate = DateTime.Now,
                    };
                    _context.SeatConfigurations.Add(newSeatConfiguration);
                }
            }

            else if (seatConfiguration != null)
            {
                seatConfiguration.DeletedBy = existingUser.UserId;
                seatConfiguration.DeletedDate = DateTime.Now;
                seatConfiguration.ModifiedBy = existingUser.UserId;
                seatConfiguration.ModifiedDate = DateTime.Now;
                _context.SeatConfigurations.Update(seatConfiguration);
            }

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            await transaction.RollbackAsync();
            throw new Exception("An error occurred while updating the user profile.", e);
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            throw new OperationCanceledException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("An unexpected error occurred while updating the user profile.", ex);
        }
    }

    public async Task<List<short>> GetAllAssignedSeats(string subjectId)
    {
        var userId = await _context.Users
                                   .AsNoTracking()
                                   .Where(u => u.SubjectId == subjectId && u.DeletedDate == null)
                                   .Select(u => u.UserId)
                                   .FirstOrDefaultAsync();

        return await _context.SeatConfigurations
                            .AsNoTracking()
                            .Where(s => s.UserId != userId && s.DeletedDate == null)
                            .Select(s => s.SeatId)
                            .ToListAsync();
    }

    public async Task<List<string?>> GetAllPhoneNumbers(string subjectId)
    {
        var userId = await _context.Users
                                   .AsNoTracking()
                                   .Where(u => u.SubjectId == subjectId && u.DeletedDate == null)
                                   .Select(u => u.UserId)
                                   .FirstOrDefaultAsync();

        return await _context.Users
                             .AsNoTracking()
                             .Where(u => u.UserId != userId && u.DeletedDate == null)
                             .Select(u => u.PhoneNumber)
                             .ToListAsync();
    }

    public async Task<Seat?> FindSeatBySeatIdAndCityIdAsync(short seatId, byte cityId)
    {
        var seat = await _context.Seats.FirstOrDefaultAsync(s => s.SeatId == seatId && s.ColumnModel!.FloorModel!.CityId == cityId);
        return seat;
    }
}
