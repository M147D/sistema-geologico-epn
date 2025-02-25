using AutoMapper;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;
using Servidor_Sistema_Geologia.Constants;

namespace Servidor_Sistema_Geologia.Infrastructure.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Mapeo de Fosil a FosilDto
			CreateMap<Fosil, FosilDto>()
				.ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Galeria != null ? src.Galeria.Fotos : null))
				.ForMember(dest => dest.Galeria, opt => opt.MapFrom(src => src.Galeria));

			// Mapeo de FosilDto a Fosil
			CreateMap<FosilDto, Fosil>()
				.ForPath(dest => dest.Galeria, opt => opt.Ignore());

			// Mapeo de Roca a RocaDto
			CreateMap<Roca, RocaDto>()
				.ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Galeria != null ? src.Galeria.Fotos : null))
				.ForMember(dest => dest.Galeria, opt => opt.MapFrom(src => src.Galeria));

			// Mapeo de RocaDto a Roca
			CreateMap<RocaDto, Roca>()
				.ForPath(dest => dest.Galeria, opt => opt.Ignore());

			// Mapeo de otras entidades
			CreateMap<Ubicacion, UbicacionDto>().ReverseMap();
			CreateMap<Pais, PaisDto>().ReverseMap();
			CreateMap<Provincia, ProvinciaDto>().ReverseMap();

			// Mapeo de EstadoElemento a EstadoElementoDto y viceversa
			CreateMap<EstadoElemento, EstadoElementoDto>()
				.ForMember(dest => dest.DescripcionEstado, opt => opt.MapFrom(src => src.DescripcionEstado.ToString()));

			CreateMap<EstadoElementoDto, EstadoElemento>()
				.ForMember(dest => dest.DescripcionEstado, opt => opt.MapFrom(src =>
					Enum.TryParse<EstadosElemento>(src.DescripcionEstado, out var estado)
						? estado
						: EstadosElemento.Creado));

			// Mapeo de GaleriaElementoGeologico a GaleriaElementoGeologicoDto y viceversa
			CreateMap<GaleriaElementoGeologico, GaleriaElementoGeologicoDto>().ReverseMap();

			// Mapeo de FotoElemento a FotoElementoDto y viceversa
			CreateMap<FotoElemento, FotoElementoDto>().ReverseMap();
		}
	}
}