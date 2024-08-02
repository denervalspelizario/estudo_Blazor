using PaginaColetaEmailVideo.Dto;
using PaginaColetaEmailVideo.Models;

namespace PaginaColetaEmailVideo.Services.UsuarioServices
{
    public interface IUsuarioInterface
    {
        Task<UsuarioModel> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto);
        Task<UsuarioModel> Login(LoginDto loginDto);
    }
}
