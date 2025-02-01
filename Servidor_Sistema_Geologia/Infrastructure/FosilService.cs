using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class FosilService : ElementoGeologicoService<Fosil, FosilDto>
	{
		private readonly IMapper _mapper;

		public FosilService(GestorGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override FosilDto ConvertToDto(Fosil fosil)
		{
			return _mapper.Map<FosilDto>(fosil);
		}

		protected override Fosil ConvertToEntity(FosilDto dto)
		{
			return _mapper.Map<Fosil>(dto);
		}

		protected override void UpdateEntity(Fosil fosil, FosilDto dto)
		{
			_mapper.Map(dto, fosil); // Aplica los cambios del DTO en la entidad existente.
		}
	}
}
