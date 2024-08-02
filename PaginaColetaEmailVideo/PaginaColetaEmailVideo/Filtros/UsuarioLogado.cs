using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaginaColetaEmailVideo.Models;

namespace PaginaColetaEmailVideo.Filtros
{
    // classe que vai fazer o filtro se user está logo
    public class UsuarioLogado : ActionFilterAttribute // herda esta classe Action
    {

        // método sobreescrito
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            // verificando se user está logado, como? so vendo se existe alguma sessão ativa
            string sessaoUsuario = context.HttpContext.Session.GetString("UsuarioAtivo");

            // não existe sessão enão sessão é null(não tem usuario logado)
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                // se sessão for null(não tem usuario logado) vou redirecionar a pagina Index do controller Home
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller", "Home" },
                    {"Action", "Index"}
                });
            }
            else // não esta logado ou seja tem alguma sessão
            {
                // fazendo a desserelização(tranformar em um usuário UsuarioModel)
                UsuarioModel usuario = JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);

                // se a dessereliazação for null (não conseguiu tranformar em algum usuario UsuarioModel)
                if (usuario == null)
                {
                    //  então tem problema e também vou redirecionar a pagina Index do controller Home
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller", "Home" },
                    {"Action", "Index"}
                });
                }
            }


            // deu tudo certo existe uma sessão e essa sessão depois de desserializada virou um UsuarioModel
            base.OnActionExecuted(context);
        }


    }
}
