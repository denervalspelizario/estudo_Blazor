using PaginaColetaEmailVideo.Models;

namespace PaginaColetaEmailVideo.Services.SessaoServices
{
    public interface ISessaoInterface
    {
        UsuarioModel BuscarSessao();
        void CriarSessao(UsuarioModel usuario);
        void RemoverSessao();

    }
}
