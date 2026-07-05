using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.DTO.Users;
using Servidor_Sistema_Geologia.Repositories.Interfaces;

namespace Servidor_Sistema_Geologia.Repositories.Implementation;

public class UserRepository : IUserRepository
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SistemaGeologicoDbContext _context;

    public UserRepository(UserManager<Usuario> userManager, SistemaGeologicoDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _userManager.FindByIdAsync(id.ToString());
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<Usuario?> GetByUserNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<PaginatedUsersDto> GetAllAsync(UserFilterDto filter)
    {
        var query = _context.Users.AsQueryable();
        
        // 🔥 POR DEFECTO: Solo mostrar usuarios activos, excepto si se especifica lo contrario
        if (!filter.IncludeInactive)
        {
            query = query.Where(u => u.EstadoActivo);
        }

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filter.Email))
        {
            query = query.Where(u => u.Email!.Contains(filter.Email));
        }

        if (!string.IsNullOrEmpty(filter.NombreCompleto))
        {
            query = query.Where(u => u.NombreCompleto!.Contains(filter.NombreCompleto));
        }

        if (filter.Rol.HasValue)
        {
            query = query.Where(u => u.Rol == filter.Rol.Value);
        }

        if (filter.EstadoActivo.HasValue)
        {
            query = query.Where(u => u.EstadoActivo == filter.EstadoActivo.Value);
        }

        if (filter.EmailConfirmed.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed == filter.EmailConfirmed.Value);
        }

        if (filter.FechaCreacionDesde.HasValue)
        {
            query = query.Where(u => u.FechaCreacion >= filter.FechaCreacionDesde.Value);
        }

        if (filter.FechaCreacionHasta.HasValue)
        {
            query = query.Where(u => u.FechaCreacion <= filter.FechaCreacionHasta.Value);
        }

        // Aplicar ordenamiento
        query = filter.SortBy?.ToLower() switch
        {
            "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "nombrecompleto" => filter.SortDescending ? query.OrderByDescending(u => u.NombreCompleto) : query.OrderBy(u => u.NombreCompleto),
            "rol" => filter.SortDescending ? query.OrderByDescending(u => u.Rol) : query.OrderBy(u => u.Rol),
            "estadoactivo" => filter.SortDescending ? query.OrderByDescending(u => u.EstadoActivo) : query.OrderBy(u => u.EstadoActivo),
            _ => filter.SortDescending ? query.OrderByDescending(u => u.FechaCreacion) : query.OrderBy(u => u.FechaCreacion)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var users = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Email = u.Email!,
                UserName = u.UserName,
                NombreCompleto = u.NombreCompleto,
                Rol = u.Rol,
                FechaCreacion = u.FechaCreacion,
                EstadoActivo = u.EstadoActivo
            })
            .ToListAsync();

        return new PaginatedUsersDto
        {
            Users = users,
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = filter.Page,
            PageSize = filter.PageSize,
            HasPrevious = filter.Page > 1,
            HasNext = filter.Page < totalPages
        };
    }

    public async Task<IdentityResult> CreateAsync(Usuario user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateAsync(Usuario user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(Usuario user)
    {
        // 🔥 SOFT DELETE: Desactivar usuario en lugar de eliminar físicamente
        user.EstadoActivo = false;
        return await _userManager.UpdateAsync(user);
    }


    public async Task<IdentityResult> ChangePasswordAsync(Usuario user, string newPassword)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUserNameAsync(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<int> GetActiveCountAsync()
    {
        return await _context.Users.CountAsync(u => u.EstadoActivo);
    }

    public async Task<Dictionary<RolUsuario, int>> GetUserCountByRoleAsync()
    {
        var counts = await _context.Users
            .GroupBy(u => u.Rol)
            .Select(g => new { Rol = g.Key, Count = g.Count() })
            .ToListAsync();

        return counts.ToDictionary(x => x.Rol, x => x.Count);
    }
}