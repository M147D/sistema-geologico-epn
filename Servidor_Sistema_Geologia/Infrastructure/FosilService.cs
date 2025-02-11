using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class FosilService : ElementoGeologicoService<Fosil, FosilReadDto, FosilCreateDto>
	{
		private readonly IMapper _mapper;

		public FosilService(GestorGeologia db, IMapper mapper) : base(db)
		{
			_mapper = mapper;
		}

		protected override FosilReadDto ConvertToDto(Fosil fosil)
		{
			return _mapper.Map<FosilReadDto>(fosil);
		}

		protected override Fosil ConvertToEntity(FosilCreateDto dto)
		{
			return _mapper.Map<Fosil>(dto);
		}

		protected override void UpdateEntity(Fosil fosil, FosilCreateDto dto)
		{
			_mapper.Map(dto, fosil);
		}
	}
}