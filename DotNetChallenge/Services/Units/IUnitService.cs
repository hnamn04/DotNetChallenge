using DotNetChallenge.DTOs.Units;

namespace DotNetChallenge.Services.Units
{
    public interface IUnitService
    {
        Task<UnitResponse> CreateAsync(CreateUnitRequest request);

        Task<IEnumerable<UnitResponse>> GetAllAsync();

        Task<UnitResponse> UpdateAsync(Guid id, UpdateUnitRequest request);

        Task DeleteAsync(Guid id);
    }
}
