using AutoMapper;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Mapeos para DTOs de lectura (EntityToDto)
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

			// Mapeos para DTOs de creación (CreateDtoToEntity)
			CreateMap<CreateFosilDto, Fosil>()
				.ForMember(dest => dest.Ubicacion, opt => opt.Ignore())
				.ForMember(dest => dest.UbicacionId, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElemento, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElementoId, opt => opt.Ignore())
				.ForMember(dest => dest.Galeria, opt => opt.Ignore())
				.ForMember(dest => dest.GaleriaElementosGeologicoId, opt => opt.Ignore());

			CreateMap<CreateRocaDto, Roca>()
				.ForMember(dest => dest.Ubicacion, opt => opt.Ignore())
				.ForMember(dest => dest.UbicacionId, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElemento, opt => opt.Ignore())
				.ForMember(dest => dest.EstadoElementoId, opt => opt.Ignore())
				.ForMember(dest => dest.Galeria, opt => opt.Ignore())
				.ForMember(dest => dest.GaleriaElementosGeologicoId, opt => opt.Ignore());

			// Mapeos para entidades relacionadas
			CreateMap<GaleriaElementoGeologico, GaleriaElementoGeologicoDto>()
				.ForMember(dest => dest.Fotos, opt => opt.MapFrom(src => src.Fotos));

			//CreateMap<FotoElemento, FotoElementoDto>().ReverseMap();

			CreateMap<Ubicacion, UbicacionDto>()
				.ForMember(dest => dest.Pais, opt => opt.MapFrom(src => src.Pais))
				.ForMember(dest => dest.Provincia, opt => opt.MapFrom(src => src.Provincia));

			CreateMap<Pais, PaisDto>().ReverseMap();
			CreateMap<Provincia, ProvinciaDto>().ReverseMap();
			CreateMap<EstadoElemento, EstadoElementoDto>().ReverseMap();
		}
	}
}