using AutoMapper;
using Servidor_Sistema_Geologia.DAL;
using Servidor_Sistema_Geologia.DTO;
using Servidor_Sistema_Geologia.Models;

namespace Servidor_Sistema_Geologia.Infrastructure
{
	public class RocaService : ElementoGeologicoService<Roca, RocaDto, RocaDto>
	{
		public RocaService(GestorSistemaGeologia db, IMapper mapper) : base(db, mapper)
		{
			// La lógica de mapeo ahora se maneja en la clase base
		}
	}
}