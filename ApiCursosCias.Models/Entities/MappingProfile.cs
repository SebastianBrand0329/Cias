using ApiCursosCias.Models.Context.Model;
using AutoMapper;

namespace ApiCursosCias.Models.Entities;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<UsuarioLogin, TrUsuarioLogin>();
		CreateMap<UsuarioLoginExtended, UsuarioLogin>();
		CreateMap<TrUsuarioLogin, UsuarioLoginExtended>();

		CreateMap<Usuario, TrUsuario>()
			.ForMember(dest =>
			dest.Usuario,
			opt => opt.MapFrom(src => src.UserName)).ReverseMap();

		CreateMap<MaRol, Maestro>()
			.ForMember(dest =>
			dest.Id,
			opt => opt.MapFrom(src => src.IdRol))
			 .ForMember(dest =>
			dest.Nombre,
			opt => opt.MapFrom(src => src.Rol));

		CreateMap<MaSede, Maestro>()
			.ForMember(dest =>
			dest.Id,
			opt => opt.MapFrom(src => src.IdSede))
			 .ForMember(dest =>
			dest.Nombre,
			opt => opt.MapFrom(src => src.Sede));

		CreateMap<MaTiemposDescuento, Maestro>()
			.ForMember(dest =>
			dest.Id,
			opt => opt.MapFrom(src => src.IdTipoComparendo));

		CreateMap<MaTipoDocumento, Maestro>()
			 .ForMember(dest =>
			 dest.Id,
			 opt => opt.MapFrom(src => src.IdTipoDocumento))
			  .ForMember(dest =>
			 dest.Nombre,
			 opt => opt.MapFrom(src => src.TipoDocumento));

		CreateMap<TrDocente, Maestro>()
				.ForMember(dest =>
				dest.Nombre,
				opt => opt.MapFrom(src => src.NombreDocente)).ReverseMap();

		CreateMap<Curso, TrCurso>()
			.ForMember(dest =>
			 dest.IdUsuarioRegistro,
			 opt => opt.MapFrom(src => src.IdUsuario)).ReverseMap();
		CreateMap<SetAgenda, TrAgendaCurso>()
			.ForMember(dest =>
			 dest.IdUsuarioRegistro,
			 opt => opt.MapFrom(src => src.IdUsuario))
			.ReverseMap();
		CreateMap<SetConsulta, TrConsultaComparendo>()
			.ForMember(dest =>
			 dest.Comparendo,
			 opt => opt.MapFrom(src => src.NumeroComparendo))
			  .ForMember(dest =>
			 dest.Descripcion,
			 opt => opt.MapFrom(src => src.Consulta)).ReverseMap();

		CreateMap<Pago, TrPagoAgendum>().ReverseMap();
		CreateMap<Notificacion, MaPlantilla>()
			.ForMember(dest =>
			dest.Descripcion,
			opt => opt.MapFrom(src => src.Asunto)).ReverseMap();

		CreateMap<TrAgendaCursoHst, TrAgendaCurso>().ReverseMap();
	}
}

public static class MapperBootstrapper
{
	private static IMapper _instance;
	public static IMapper Instance => _instance;

	public static void Configure()
	{
		if (_instance == null)
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<MappingProfile>();
			});
			_instance = config.CreateMapper();
		}
	}
}