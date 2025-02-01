using AutoMapper;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Mapeo de Fosil a FosilDto
			CreateMap<Fosil, FosilDto>()
				.ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Fotos.Select(f => new FotoElementoDto { Imagen = f.Imagen }).ToList()))
				.ForMember(dest => dest.Ubicacion, opt => opt.MapFrom(src => src.Ubicacion))
				.ForMember(dest => dest.EstadoElemento, opt => opt.MapFrom(src => src.EstadoElemento));

			// Mapeo de FosilDto a Fosil
			CreateMap<FosilDto, Fosil>()
				.ForMember(dest => dest.Fotos, opt => opt.Ignore()) // Ignorar si no quieres sobrescribir
				.ForMember(dest => dest.Ubicacion, opt => opt.Ignore()) // Puedes configurarlo según tu lógica
				.ForMember(dest => dest.EstadoElemento, opt => opt.Ignore());

			// Mapeo de Ubicacion a UbicacionDto y viceversa
			CreateMap<Ubicacion, UbicacionDto>().ReverseMap();
			CreateMap<Pais, PaisDto>().ReverseMap();
			CreateMap<Provincia, ProvinciaDto>().ReverseMap();
			CreateMap<EstadoElemento, EstadoElementoDto>().ReverseMap();
		}
	}
}
