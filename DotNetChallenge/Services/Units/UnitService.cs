using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Units;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Units
{
    public class UnitService : IUnitService
    {
        private readonly AppDbContext _context;

        public UnitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UnitResponse> CreateAsync(CreateUnitRequest request)
        {
            var unit = new Unit
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Symbol = request.Symbol.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Units.Add(unit);

            await _context.SaveChangesAsync();

            return MapToResponse(unit);
        }

        public async Task<IEnumerable<UnitResponse>> GetAllAsync()
        {
            var units = await _context.Units
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return units.Select(MapToResponse);
        }

        public async Task<UnitResponse> UpdateAsync(Guid id, UpdateUnitRequest request)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(x => x.Id == id);

            if (unit is null)
            {
                throw new NotFoundException(
                    $"Unit with id '{id}' was not found.");
            }

            unit.Name = request.Name.Trim();
            unit.Symbol = request.Symbol.Trim();
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(unit);
        }

        public async Task DeleteAsync(Guid id)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(x => x.Id == id);

            if (unit is null)
            {
                throw new NotFoundException(
                    $"Unit with id '{id}' was not found.");
            }

            var hasProducts = await _context.Products
                .AnyAsync(x => x.UnitId == id);

            if (hasProducts)
            {
                throw new ConflictException(
                    "Cannot delete unit because it is being used by one or more products.");
            }

            _context.Units.Remove(unit);

            await _context.SaveChangesAsync();
        }

        private static UnitResponse MapToResponse(Unit unit)
        {
            return new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Symbol = unit.Symbol,
                CreatedAt = unit.CreatedAt,
                UpdatedAt = unit.UpdatedAt
            };
        }
    }
}
