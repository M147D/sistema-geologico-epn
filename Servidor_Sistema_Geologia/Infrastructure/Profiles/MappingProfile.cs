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
				.ForMember(dest => dest.Galeria, opt => opt.MapFrom(src =>
					src.Galeria != null ? new GaleriaElementoGeologicoDto
					{
						Id = src.Galeria.Id,
						DetalleGrupo = src.Galeria.DetalleGrupo,
						Fotos = src.Galeria.Fotos.Select(f => new FotoElementoDto
						{
							Id = f.Id,
							Imagen = f.Imagen,
							TipoFoto = f.TipoFoto,
							FechaSubida = f.FechaSubida,
							CreadoPor = f.CreadoPor,
							DescripcionEspecifica = f.DescripcionEspecifica,
							Etiquetas = f.Etiquetas
						}).ToList()
					} : null
				))
				.ForMember(dest => dest.Ubicacion, opt => opt.MapFrom(src => src.Ubicacion))
				.ForMember(dest => dest.EstadoElemento, opt => opt.MapFrom(src => src.EstadoElemento));

			// Mapeo de FosilDto a Fosil
			CreateMap<FosilDto, Fosil>()
				.ForMember(dest => dest.Galeria, opt => opt.Ignore())
				.ForMember(dest => dest.Ubicacion, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElemento, opt => opt.Ignore());

			// Mapeos adicionales necesarios
			CreateMap<GaleriaElementoGeologico, GaleriaElementoGeologicoDto>()
				.ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Fotos));
			CreateMap<FotoElemento, FotoElementoDto>().ReverseMap();

			// Mapeo de Ubicacion, Pais, Provincia, EstadoElemento
			CreateMap<Ubicacion, UbicacionDto>().ReverseMap();
			CreateMap<Pais, PaisDto>().ReverseMap();
			CreateMap<Provincia, ProvinciaDto>().ReverseMap();
			CreateMap<EstadoElemento, EstadoElementoDto>().ReverseMap();

			// Mapeo de Roca a RocaDto
			CreateMap<Roca, RocaDto>()
				.ForMember(dest => dest.Galeria, opt => opt.MapFrom(src =>
					src.Galeria != null ? new GaleriaElementoGeologicoDto
					{
						Id = src.Galeria.Id,
						DetalleGrupo = src.Galeria.DetalleGrupo,
						Fotos = src.Galeria.Fotos.Select(f => new FotoElementoDto
						{
							Id = f.Id,
							Imagen = f.Imagen,
							TipoFoto = f.TipoFoto,
							FechaSubida = f.FechaSubida,
							CreadoPor = f.CreadoPor,
							DescripcionEspecifica = f.DescripcionEspecifica,
							Etiquetas = f.Etiquetas
						}).ToList()
					} : null
				))
				.ForMember(dest => dest.Ubicacion, opt => opt.MapFrom(src => src.Ubicacion))
				.ForMember(dest => dest.EstadoElemento, opt => opt.MapFrom(src => src.EstadoElemento));

			// Mapeo de RocaDto a Roca
			CreateMap<RocaDto, Roca>()
				.ForMember(dest => dest.Galeria, opt => opt.Ignore())
				.ForMember(dest => dest.Ubicacion, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElemento, opt => opt.Ignore());
		}
	}
}