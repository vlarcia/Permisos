using Entity;
using System.Globalization;
using AutoMapper;
using Permisos.WebApp.Models.ViewModels;


namespace Permisos.WebApp.Utilidades.Automapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<TbRol,VMRol>().ReverseMap();
            #endregion Rol

            #region Usuario
            CreateMap<TbUsuario, VMUsuario>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                 .ForMember(dest => dest.NombreArea, opt => opt.MapFrom(src => src.IdAreaNavigation.Nombre))
                .ForMember(destino => destino.NombreRol, opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            CreateMap<VMUsuario, TbUsuario>()
                .ForMember(destino => destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdRolNavigation, opt => opt.Ignore())
                .ForMember(destino => destino.IdAreaNavigation, opt => opt.Ignore());

            #endregion Usuario

            #region Negocio
            //CreateMap<Negocio, VMNegocio>()
            //    .ForMember(destino => destino.PorcentajeImpuesto,
            //    opt=> opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-NI"))));
            CreateMap<TbNegocio, VMNegocio>().ReverseMap();
            #endregion Negocio


            #region Menu
            CreateMap<TbMenu, VMMenu>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion 

            #region PDF
            CreateMap<TbMenu, VMMenu>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion


            #region Permisos
            CreateMap<TbPermiso, VMPermiso>()
                .ForMember(dest => dest.Alertas, opt => opt.MapFrom(src => src.TbAlerta))  // Mapea la colección de alertas
                .ForMember(dest => dest.NombreArea, opt => opt.MapFrom(src => src.IdAreaNavigation.Nombre))
                .ForMember(dest => dest.PermisosPorDestinatarios, opt => opt.MapFrom(src => src.TbPermisoDestinatarios)) // Lista de permisos asociados al 
                .ReverseMap()
                .ForMember(dest => dest.IdAreaNavigation, opt => opt.Ignore());
            #endregion Permisos

            #region Destinatario

            CreateMap<TbDestinatario, VMDestinatario>()
                .ForMember(dest => dest.Alertas, opt => opt.MapFrom(src => src.TbAlerta))
                .ForMember(dest => dest.NombreArea, opt => opt.MapFrom(src => src.IdAreaNavigation != null ? src.IdAreaNavigation.Nombre : string.Empty))
                .ForMember(dest => dest.PermisosPorDestinatarios, opt => opt.MapFrom(src => src.TbPermisoDestinatarios));

            CreateMap<VMDestinatario, TbDestinatario>()
                .ForMember(dest => dest.TbAlerta, opt => opt.Ignore()) // Ignorar colecciones que no se van a modificar
                .ForMember(dest => dest.IdAreaNavigation, opt => opt.Ignore()) // Ignorar navegación si viene null
                .ForMember(dest => dest.TbPermisoDestinatarios, opt => opt.MapFrom(src => src.PermisosPorDestinatarios));

            #endregion


            #region Alerta
            CreateMap<TbAlerta, VMAlerta>()                 
                .ForMember(dest => dest.Permiso, opt => opt.MapFrom(src => src.IdPermisoNavigation.Nombre))
                .ForMember(dest => dest.Destinatario, opt => opt.MapFrom(src => src.IdDestinatarioNavigation.Nombre))
                .ReverseMap()
                .ForMember(dest => dest.IdPermisoNavigation, opt => opt.Ignore())  // Ignorar IdPermisoNavigation para evitar crear una nueva finca                
                .ForMember(dest => dest.IdDestinatarioNavigation, opt => opt.Ignore());  // Ignorar IdDestinatarioNavigation para evitar crear una nueva finca                

            #endregion Alerta

            CreateMap<TbArea, VMArea>().ReverseMap();

            CreateMap<TbPermisoDestinatario, VMPermisoDestinatario>()
                .ForMember(dest => dest.NombrePermiso,    opt => opt.MapFrom(src => src.IdPermisoNavigation != null ? src.IdPermisoNavigation.Nombre : ""))
                .ForMember(dest => dest.Institucion,      opt => opt.MapFrom(src => src.IdPermisoNavigation != null ? src.IdPermisoNavigation.Institucion : ""))
                .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.IdPermisoNavigation.FechaVencimiento))
                .ReverseMap()
                .ForMember(dest => dest.IdDestinatarioNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdPermisoNavigation, opt => opt.Ignore());

        }
    }
}
