using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;

namespace Business.Implementacion
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<TbMenu> _repositorioMenu;
        private readonly IGenericRepository<TbRolMenu> _repositorioRolMenu;
        private readonly IGenericRepository<TbUsuario> _repositorioUsuario;
        public MenuService(IGenericRepository<TbMenu> repositorioMenu, IGenericRepository<TbRolMenu> repositorioRolMenu, IGenericRepository<TbUsuario> repositorioUsuario)
        {
            _repositorioMenu = repositorioMenu;
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<List<TbMenu>> ObtenerMenus(int idusuario)
        {
            IQueryable<TbUsuario> tbUsuario= _repositorioUsuario.Consultar(u => u.IdUsuario == idusuario);
            IQueryable<TbRolMenu> tbRolMenu = _repositorioRolMenu.Consultar();
            IQueryable<TbMenu> tbMenu=  _repositorioMenu.Consultar();

            IQueryable<TbMenu> MenuPadre=(from u in tbUsuario
                                        join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                        join m in tbMenu on rm.IdMenu equals m.IdMenu
                                        join mpadre in tbMenu on m.IdMenuPadre equals mpadre.IdMenu
                                        select mpadre).Distinct().AsQueryable();

            IQueryable<TbMenu> MenuHijos= (from u in tbUsuario
                                         join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                         join m in tbMenu on rm.IdMenu equals m.IdMenu
                                         where m.IdMenu != m.IdMenuPadre
                                         select m).Distinct().AsQueryable();

            List<TbMenu> listaMenu = (from mpadre in MenuPadre
                                    select new TbMenu()
                                    {
                                        Descripcion = mpadre.Descripcion,
                                        Icono = mpadre.Icono,
                                        Controlador = mpadre.Controlador,
                                        PaginaAccion = mpadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = (from mhijo in MenuHijos
                                                                        where mhijo.IdMenuPadre == mpadre.IdMenu
                                                                        select mhijo).ToList()
                                    }).ToList();
            return listaMenu;   
        }
    }
}
