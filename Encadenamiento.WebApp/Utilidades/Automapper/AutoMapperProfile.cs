using Encadenamiento.WebApp.Models.ViewModels;
using Entity;
using System.Globalization;
using AutoMapper;


namespace Encadenamiento.WebApp.Utilidades.Automapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol,VMRol>().ReverseMap();
            #endregion Rol

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino =>
                destino.NombreRol,
                opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            CreateMap<VMUsuario, Usuario>()
                .ForMember(destino => destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdRolNavigation,
                opt => opt.Ignore());

            #endregion Usuario

            #region Negocio
            //CreateMap<Negocio, VMNegocio>()
            //    .ForMember(destino => destino.PorcentajeImpuesto,
            //    opt=> opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-NI"))));
            CreateMap<Negocio, VMNegocio>().ReverseMap();
            #endregion Negocio

            #region Finca
            CreateMap<MaestroFinca, VMMaestroFinca>().ReverseMap();
            #endregion Finca

            #region Checklist
            CreateMap<CheckList, VMCheckList>().ReverseMap();
            #endregion Checklist

            #region Revisiones
            CreateMap<Revisione, VMRevisiones>()
                .ForMember(dest => dest.NombreFinca, opt => opt.MapFrom(orig => orig.IdFincaNavigation.Descripcion))
                .ForMember(dest => dest.Proveedor, opt => opt.MapFrom(orig => orig.IdFincaNavigation.Proveedor))
                .ForMember(dest => dest.CodFinca, opt => opt.MapFrom(orig => orig.IdFincaNavigation.CodFinca))
                .ForMember(dest => dest.Requisito, opt => opt.MapFrom(orig => orig.IdRequisitoNavigation.Descripcion))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.IdFincaNavigation.Email))
                .ForMember(dest => dest.Ambito, opt => opt.MapFrom(orig => orig.IdRequisitoNavigation.Ambito))
                .ReverseMap()
                .ForMember(dest => dest.IdFincaNavigation, opt => opt.Ignore())  // Ignorar IdFincaNavigation para evitar crear una nueva finca
                .ForMember(dest => dest.IdRequisitoNavigation, opt => opt.Ignore());  // Ignorar IdRequisitoNaviigation para evitar crear un nuevo plan
            #endregion Revisiones


            #region PlanesTrabajo
            CreateMap<PlanesTrabajo, VMPlanesTrabajo>()
                .ForMember(dest => dest.Actividades, opt => opt.MapFrom(src => src.Actividades))  // Mapea la colección de actividades y otros datos               
                .ForMember(dest => dest.NombreFinca, opt => opt.MapFrom(src => src.IdFincaNavigation.Descripcion))
                .ForMember(dest => dest.CodFinca, opt => opt.MapFrom(src => src.IdFincaNavigation.CodFinca))
                .ForMember(dest => dest.Proveedor, opt => opt.MapFrom(src => src.IdFincaNavigation.Proveedor))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.IdFincaNavigation.Email))                
                .ReverseMap()
                .ForMember(dest => dest.IdFincaNavigation, opt => opt.Ignore());  // Ignorar IdFincaNavigation para evitar crear una nueva finca                
                                                                                 // Permite el mapeo en ambas direcciones
                                                                                            
            CreateMap<Actividad, VMActividad>()
                    // Mapeo para mostrar NombreFinca, NombrePlan y FechaPlanIni
                .ForMember(dest => dest.NombreFinca, opt => opt.MapFrom(src => src.IdFincaNavigation.Descripcion))
                .ForMember(dest => dest.NombrePlan, opt => opt.MapFrom(src => src.IdPlanNavigation.Descripcion))
                .ForMember(dest => dest.FechaPlanIni, opt => opt.MapFrom(src => src.IdPlanNavigation.FechaIni))

                // Evitar que AutoMapper intente mapear las propiedades de navegación al hacer el mapeo inverso
                .ReverseMap()
                .ForMember(dest => dest.IdFincaNavigation, opt => opt.Ignore())  // Ignorar IdFincaNavigation para evitar crear una nueva finca
                .ForMember(dest => dest.IdPlanNavigation, opt => opt.Ignore());  // Ignorar IdPlanNavigation para evitar crear un nuevo plan

            #endregion 

            #region Visitas
            CreateMap<Visita, VMVisita>()
                .ForMember(dest=> dest.DetalleVisita, opt=> opt.MapFrom(src=>src.DetalleVisita))// Mapea la coleccion de detalles de la visita
                .ForMember(dest=> dest.DescripcionPlan, opt => opt.MapFrom(src => src.IdPlanNavigation.Descripcion))
                .ForMember(dest => dest.NombreFinca,  opt => opt.MapFrom(src => src.IdFincaNavigation.Descripcion))
                .ForMember(dest => dest.CodFinca, opt => opt.MapFrom(src => src.IdFincaNavigation.CodFinca))
                .ForMember(dest => dest.Proveedor, opt => opt.MapFrom(src => src.IdFincaNavigation.Proveedor))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.IdFincaNavigation.Email))
                .ReverseMap()
                .ForMember(dest => dest.IdFincaNavigation, opt => opt.Ignore())     
                .ForMember(dest => dest.IdPlanNavigation, opt => opt.Ignore());

            CreateMap<DetalleVisita, VMDetalleVisita>()
               // Mapeo para mostrar NombreFinca, NombrePlan y FechaPlanIni
               .ForMember(dest => dest.DescripcionActividad, opt => opt.MapFrom(src => src.IdActividadNavigation.Descripcion))
               .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.IdActividadNavigation.Tipo))
               .ForMember(dest => dest.FechaInicia, opt => opt.MapFrom(src => src.IdActividadNavigation.FechaIni))
               .ForMember(dest => dest.FechaFinaliza, opt => opt.MapFrom(src => src.IdActividadNavigation.FechaFin))
               .ForMember(dest => dest.FechaUltimarevision, opt => opt.MapFrom(src => src.IdActividadNavigation.FechaUltimarevision))

               // Evitar que AutoMapper intente mapear las propiedades de navegación al hacer el mapeo inverso
               .ReverseMap()
               .ForMember(dest => dest.IdFincaNavigation, opt => opt.Ignore())  // Ignorar IdFincaNavigation para evitar crear una nueva finca
               .ForMember(dest => dest.IdActividadNavigation, opt => opt.Ignore());  // Ignorar IdPlanNavigation para evitar crear un nuevo plan

#endregion

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion 

            #region PDF
            CreateMap<Menu, VMMenu>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion 

        }
    }
}
