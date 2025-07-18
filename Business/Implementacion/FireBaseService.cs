﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using Entity;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Business.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<TbConfiguracion> _repositorio;

        public FireBaseService(IGenericRepository<TbConfiguracion> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<TbConfiguracion> query = _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
                Dictionary<String, string> Config = await query.ToDictionaryAsync(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation= new CancellationTokenSource();
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;                
            }
           
            catch
            {
                UrlImagen = "";
            }
            return UrlImagen;
        }
        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<TbConfiguracion> query = _repositorio.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
                Dictionary<String, string> Config = await query.ToDictionaryAsync(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync();

                await task;
                return true;
            }

            catch
            {
                return false;
            }
        }

        
    }
}
