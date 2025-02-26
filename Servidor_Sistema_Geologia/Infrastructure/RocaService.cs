using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class RocaService : ElementoGeologicoService<Roca, RocaDto, RocaDto>
	{
		private readonly IMapper _mapper;

		public RocaService(GestorSistemaGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override RocaDto ConvertToDto(Roca roca)
		{
			return _mapper.Map<RocaDto>(roca);
		}

		protected override Roca ConvertToEntity(RocaDto dto)
		{
			var roca = new Roca
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
				TipoRoca = dto.TipoRoca,
				Litologia = dto.Litologia
				// Las relaciones se manejan en CreateAsync y UpdateAsync
			};

			return roca;
		}

		protected override void UpdateEntity(Roca roca, RocaDto dto)
		{
			// Actualizar propiedades básicas
			roca.Nombre = dto.Nombre;
			roca.Edad = dto.Edad;
			roca.Donante = dto.Donante;
			roca.FechaIngreso = dto.FechaIngreso;
			roca.Codigo = dto.Codigo;
			roca.Ejemplares = dto.Ejemplares;
			roca.DocumentosRelacionados = dto.DocumentosRelacionados;
			roca.LaminaURL = dto.LaminaURL;
			roca.LaminaExiste = dto.LaminaExiste;

			// Propiedades específicas de Roca
			roca.TipoRoca = dto.TipoRoca;
			roca.Litologia = dto.Litologia;
		}
	}
}