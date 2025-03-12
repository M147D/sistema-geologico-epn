using AutoMapper;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.DTO;

namespace Servidor_Sistema_Geologia.Infrastructure.Profiles
{
	public class FotoElementoProfile : Profile
	{
		public FotoElementoProfile()
		{
			// FotoElemento -> FotoElementoDto
			CreateMap<FotoElemento, FotoElementoDto>()
				.ForMember(dest => dest.Galeria, opt => opt.Ignore()); // No navegar por relaciones

			// CreateFotoElementoDto -> FotoElemento
			CreateMap<CreateFotoElementoDto, FotoElemento>()
				.ForMember(dest => dest.Galeria, opt => opt.Ignore())
				.ForMember(dest => dest.GaleriaElementosGeologicoId, opt => opt.Ignore())
				.ForMember(dest => dest.FechaSubida, opt => opt.Ignore())
				.ForMember(dest => dest.CreadoPor, opt => opt.Ignore());
		}
	}
}