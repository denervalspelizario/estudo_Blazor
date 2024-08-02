using Newtonsoft.Json;
using PaginaColetaEmailVideo.Models;
using Microsoft.AspNetCore.Http;

namespace PaginaColetaEmailVideo.Services.SessaoServices
{
    public class SessaoService : ISessaoInterface
    {
        // acessando nosso httAcessor, configurar em porgram.cs atenção
        private readonly IHttpContextAccessor _httpAcessor;
        
        public SessaoService(IHttpContextAccessor httpAcessor)
        {
            _httpAcessor = httpAcessor;
        }


        public UsuarioModel BuscarSessao()
        {
            // acessando(pegando) a sessão(UsuarioAtivo)
            string sessaoUsuario = _httpAcessor.HttpContext.Session.GetString("UsuarioAtivo");
            
            // se a sessão não existir(ou seja não tem usuario logado)
            if (sessaoUsuario == null)
            {
                // retorna null
                return null;
            }

            // se existir então vamos deserializar e transformar o json(sessãoUsuario)
            // em objeto(UsuarioModel)
            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
        }

        public void CriarSessao(UsuarioModel usuario)
        {
            
            //JsonConvert precisa baixar Nugget Newtonsoft.json
            // transformando usuario em json
            string usuarioJson = JsonConvert.SerializeObject(usuario);

            // setando uma sessão que será chamada de UsuarioAtivo e dentro dessa sessão terá
            // o usuarioJson que foi o usuario tranformando em json
            _httpAcessor.HttpContext.Session.SetString("UsuarioAtivo", usuarioJson);
            
        }

        public void RemoverSessao()
        {
            // acessando a sessão e removendo ela(UsuarioAtivo)
            _httpAcessor.HttpContext.Session.Remove("UsuarioAtivo");
        }
    }
}
