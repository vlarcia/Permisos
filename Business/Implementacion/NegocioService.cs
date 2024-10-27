using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace Business.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;        
        private readonly IFireBaseService _firebaseService;
            
        public NegocioService(IGenericRepository<Negocio> repositorio, IFireBaseService fireBaseService)
        {
            _repositorio = repositorio;
            _firebaseService = fireBaseService;

        }
        public async Task<Negocio> Obtener()
        {

            try
            {
                Negocio negocio_encontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocio_encontrado;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }
        public async  Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
        {

            try
            {
                Negocio negocio_encontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);

                negocio_encontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocio_encontrado.Nombre=entidad.Nombre;
                negocio_encontrado.Correo=entidad.Correo;
                negocio_encontrado.Direccion=entidad.Direccion;               
                negocio_encontrado.Telefono=entidad.Telefono;
                negocio_encontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocio_encontrado.SimboloMoneda=entidad.SimboloMoneda;

                negocio_encontrado.NombreLogo = NombreLogo == "" ? negocio_encontrado.NombreLogo : NombreLogo;

                if (Logo != null)
                {
                    string urlLogo = await _firebaseService.SubirStorage(Logo, "carpeta_logo", negocio_encontrado.NombreLogo);
                    negocio_encontrado.UrlLogo = urlLogo;
                }

                await _repositorio.Editar(negocio_encontrado);
                return negocio_encontrado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

      

    }
}
