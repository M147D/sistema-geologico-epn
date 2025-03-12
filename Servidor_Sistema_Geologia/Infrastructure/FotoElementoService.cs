using Microsoft.EntityFrameworkCore;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Application;
using Servidor_Sistema_Geologia.DAL;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Google;

namespace Servidor_Sistema_Geologia.Services
{
	public class FotoElementoService : IFotoService<FotoElemento, FotoElementoDto, CreateFotoElementoDto>
	{
		private readonly GestorSistemaGeologia _context;
		private readonly IMapper _mapper;

		public FotoElementoService(GestorSistemaGeologia context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<IEnumerable<FotoElementoDto>> GetAllAsync()
		{
			var fotos = await _context.FotosElementos
				.Include(f => f.Galeria)
				.ThenInclude(g => g.ElementoGeologico)
				.ToListAsync();

			return _mapper.Map<IEnumerable<FotoElementoDto>>(fotos);
		}

		public async Task<FotoElementoDto> GetByIdAsync(int id)
		{
			var foto = await _context.FotosElementos
				.Include(f => f.Galeria)
				.ThenInclude(g => g.ElementoGeologico)
				.FirstOrDefaultAsync(f => f.Id == id);

			if (foto == null)
				throw new KeyNotFoundException($"Foto con ID {id} no encontrada");

			return _mapper.Map<FotoElementoDto>(foto);
		}

		public async Task<FotoElemento> CreateAsync(CreateFotoElementoDto fotoDto, int galeriaId, string userName)
		{
			var galeria = await _context.GaleriaElementosGeologicos
				.FirstOrDefaultAsync(g => g.Id == galeriaId);

			if (galeria == null)
				throw new KeyNotFoundException($"Galería con ID {galeriaId} no encontrada");

			var foto = _mapper.Map<FotoElemento>(fotoDto);
			foto.GaleriaElementosGeologicoId = galeriaId;
			foto.FechaSubida = DateTime.Now;
			foto.CreadoPor = userName;

			await _context.FotosElementos.AddAsync(foto);
			await _context.SaveChangesAsync();

			return foto;
		}

		public async Task<FotoElemento> UpdateAsync(int id, CreateFotoElementoDto dto, int usuarioId)
		{
			var foto = await _context.FotosElementos.FindAsync(id);
			if (foto == null)
				throw new KeyNotFoundException($"Foto con ID {id} no encontrada");

			// Update properties from DTO
			foto.TipoFoto = dto.TipoFoto;
			foto.DescripcionEspecifica = dto.DescripcionEspecifica;
			foto.Etiquetas = dto.Etiquetas;

			// Only update image if provided
			if (dto.Imagen != null && dto.Imagen.Length > 0)
			{
				foto.Imagen = dto.Imagen;
			}

			_context.FotosElementos.Update(foto);
			await _context.SaveChangesAsync();

			return foto;
		}

		public async Task DeleteAsync(int id, int usuarioId)
		{
			var foto = await _context.FotosElementos.FindAsync(id);
			if (foto == null)
				throw new KeyNotFoundException($"Foto con ID {id} no encontrada");

			_context.FotosElementos.Remove(foto);
			await _context.SaveChangesAsync();
		}
	}
}