using DotNetChallenge.Data;
using DotNetChallenge.DTOs.Roles;
using DotNetChallenge.Exceptions;
using DotNetChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        // Get all roles
        public async Task<List<RoleResponse>> GetAllAsync()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return roles
                .Select(x => new RoleResponse
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        // Assign a role to a user
        public async Task AssignRoleAsync(AssignRoleRequest request)
        {
            // Check user
            var userExists = await _context.Users
                .AnyAsync(x => x.Id == request.UserId);

            // If the user does not exist, throw a NotFoundException
            if (!userExists)
            {
                throw new NotFoundException($"User with id '{request.UserId}' was not found.");
            }

            // Check role
            var roleExists = await _context.Roles
                .AnyAsync(x => x.Id == request.RoleId);

            // If the role does not exist, throw a NotFoundException
            if (!roleExists)
            {
                throw new NotFoundException($"Role with id '{request.RoleId}' was not found.");
            }

            // Check duplicate role
            var alreadyAssigned = await _context.UserRoles
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.RoleId == request.RoleId);

            // If the role is already assigned to the user, throw a ConflictException
            if (alreadyAssigned)
            {
                throw new ConflictException("This role is already assigned to the user.");
            }

            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            };

            _context.UserRoles.Add(userRole);

            await _context.SaveChangesAsync();
        }
    }
}
