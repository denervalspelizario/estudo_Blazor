using Microsoft.AspNetCore.Mvc;
using PaginaColetaEmailVideo.Dto;
using PaginaColetaEmailVideo.Models;
using PaginaColetaEmailVideo.Services.EmailServices;
using PaginaColetaEmailVideo.Services.SessaoServices;
using PaginaColetaEmailVideo.Services.UsuarioServices;
using System.Diagnostics;

namespace PaginaColetaEmailVideo.Controllers
{
    public class HomeController : Controller
    {

        // inje��es de dependencia acessando os m�todos para realizar as requisi��es
        private readonly IEmailInterface _emailInterface;
        private readonly IUsuarioInterface _usuarioInterface;
        private readonly ISessaoInterface _sessaoInterface;

        public HomeController(IEmailInterface emailInterface, IUsuarioInterface usuarioInterface, ISessaoInterface sessaoInterface)
        {
            _emailInterface = emailInterface;
            _usuarioInterface = usuarioInterface;
            _sessaoInterface = sessaoInterface;
        }




        // chama a view a pagina de entrada que � o index    
        public IActionResult Index()
        {
            return View();
        }

		// chama a view a pagina de agradecimento     
		public IActionResult Agradecimento(EmailModel InfoRecebida)
		{
			return View(InfoRecebida);
		}

        // chama a view da pagina de Login
        public IActionResult Login()
        {
            return View();
        }

        // desloga(termina a sess�o) e redireciona a pagina Index de Home
        public IActionResult Sair()
        {
            // chama o m�todo que termina a sess�o
            _sessaoInterface.RemoverSessao();
            
            // redireciona a pagina Index de Home
            return RedirectToAction("Index");
        }


        // Metodo Post que salva os dados do cliente
        [HttpPost]

        public async Task<ActionResult> SalvarDadosCliente(EmailModel InfoRecebida)
        {
            // validando se dados foram preenchidos corretamente e est�o validos
            if (ModelState.IsValid)
            {
                // chamando o  m�todo SalvarDadosCliente para salvar dados do cliente
                var registroFeito = await _emailInterface.SalvarDadosCliente(InfoRecebida);

                // deu certo retorna a pagina Agradecimento com a resposta da requisi��o(regristroFeito)
                return View("Agradecimento", registroFeito);
            }
            else
            {   // tendo erro retorna para pagina principal Index com os erros
                return RedirectToAction("Index");
            }

        }


        // m�todo post que faz o login do usu�rio
        [HttpPost]

        public async Task<ActionResult<UsuarioModel>> Login(LoginDto loginDto)
        {
            // esta tudo valido no modelo?
            if (ModelState.IsValid)
            {
                // se estiver acessa o metodo login usando o parametro passado no m�todo
                var usuario = await _usuarioInterface.Login(loginDto);

                // n�o achou usu�rio
                if (usuario.Id == 0)
                {
                    // msg de erro
                    TempData["MensagemErro"] = "Credenciais inv�lidas!";
                    
                    // retorna a view de home com os dados de logindto
                    return View(loginDto);
                }
                else
                {
                    // existe o usus�rio manda msg de sucesso
                    TempData["MensagemSucesso"] = "Usu�rio logado com sucesso!";

                    // criando a sess�o atravez do m�todo CriarSessao usado os dados usuario
                    _sessaoInterface.CriarSessao(usuario);

                    // redireciona a pagina  Index da view de  Email
                    return RedirectToAction("Index", "Email");
                }

            }
            else
            {
                // se n�o estiver retorna a view de Home com os dados de logindto
                return View(loginDto);
            }


        }
    }
}
