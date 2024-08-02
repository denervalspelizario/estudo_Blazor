using Microsoft.AspNetCore.Mvc;
using PaginaColetaEmailVideo.Dto;
using PaginaColetaEmailVideo.Filtros;
using PaginaColetaEmailVideo.Models;
using PaginaColetaEmailVideo.Services.UsuarioServices;

namespace PaginaColetaEmailVideo.Controllers
{
    // apenas se usuarios logados podem acessar os métodos e views de EmailController
    [UsuarioLogado]
    public class UsuarioController : Controller
    {
        // injeção de dependencia para chamar a interface _usuarioInterface
        private readonly IUsuarioInterface _usuarioInterface;
        public UsuarioController(IUsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }


        // chama a view a pagina Cadastrar
        public IActionResult Cadastrar()
        {
            return View();
        }


        // método post que cadastra usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioModel>> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            // usuario mandou os dados
            if (ModelState.IsValid)
            {
                // se mandou cadastra usuario
                var usuario = await _usuarioInterface.Cadastrar(usuarioCriacaoDto);

                // deu certo? cadastrou usuario então
                if (usuario != null)
                {
                    // mensagem de sucesso
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";

                    // se deu certo além da msg de sucesso vai redirecionar de tela para
                    // tela de Index do controller Email
                    return RedirectToAction("Index", "Email");
                }
                else
                {
                    // deu errado usuario não foi cadastrado
                    TempData["MensagemErro"] = "Ocorreu um erro no momento do cadastro!";

                    // além da mensagem de erro retorna a para pagina View de criação do usuario
                    return View(usuarioCriacaoDto);
                }

            }
            else // usuario não mandou os dados então retorna para view(Cadastrar)
            {
                return View(usuarioCriacaoDto);
            }
        }
    }
}
