using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class FosilService : ElementoGeologicoService<Fosil, FosilDto, FosilDto>
	{
		private readonly IMapper _mapper;

		public FosilService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override FosilDto ConvertToDto(Fosil fosil)
		{
			return _mapper.Map<FosilDto>(fosil);
		}

		protected override Fosil ConvertToEntity(FosilDto dto)
		{
			var fosil = new Fosil
			{
				Id = dto.Id,
				Nombre = dto.Nombre,
				Edad = dto.Edad,
				Donante = dto.Donante,
				FechaIngreso = dto.FechaIngreso,
				Codigo = dto.Codigo,
				Ejemplares = dto.Ejemplares,
				DocumentosRelacionados = dto.DocumentosRelacionados,
				LaminaURL = dto.LaminaURL,
				LaminaExiste = dto.LaminaExiste,
				Especie = dto.Especie,
				Periodo = dto.Periodo
				// Las relaciones se manejan en CreateAsync y UpdateAsync
			};

			return fosil;
		}

		protected override void UpdateEntity(Fosil fosil, FosilDto dto)
		{
			// Actualizar propiedades básicas
			fosil.Nombre = dto.Nombre;
			fosil.Edad = dto.Edad;
			fosil.Donante = dto.Donante;
			fosil.FechaIngreso = dto.FechaIngreso;
			fosil.Codigo = dto.Codigo;
			fosil.Ejemplares = dto.Ejemplares;
			fosil.DocumentosRelacionados = dto.DocumentosRelacionados;
			fosil.LaminaURL = dto.LaminaURL;
			fosil.LaminaExiste = dto.LaminaExiste;

			// Propiedades específicas de Fosil
			fosil.Especie = dto.Especie;
			fosil.Periodo = dto.Periodo;
		}
	}
}