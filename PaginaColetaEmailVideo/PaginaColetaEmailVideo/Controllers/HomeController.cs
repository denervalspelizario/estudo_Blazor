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

        // injeções de dependencia acessando os métodos para realizar as requisições
        private readonly IEmailInterface _emailInterface;
        private readonly IUsuarioInterface _usuarioInterface;
        private readonly ISessaoInterface _sessaoInterface;

        public HomeController(IEmailInterface emailInterface, IUsuarioInterface usuarioInterface, ISessaoInterface sessaoInterface)
        {
            _emailInterface = emailInterface;
            _usuarioInterface = usuarioInterface;
            _sessaoInterface = sessaoInterface;
        }




        // chama a view a pagina de entrada que é o index    
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

        // desloga(termina a sessão) e redireciona a pagina Index de Home
        public IActionResult Sair()
        {
            // chama o método que termina a sessão
            _sessaoInterface.RemoverSessao();
            
            // redireciona a pagina Index de Home
            return RedirectToAction("Index");
        }


        // Metodo Post que salva os dados do cliente
        [HttpPost]

        public async Task<ActionResult> SalvarDadosCliente(EmailModel InfoRecebida)
        {
            // validando se dados foram preenchidos corretamente e estão validos
            if (ModelState.IsValid)
            {
                // chamando o  método SalvarDadosCliente para salvar dados do cliente
                var registroFeito = await _emailInterface.SalvarDadosCliente(InfoRecebida);

                // deu certo retorna a pagina Agradecimento com a resposta da requisição(regristroFeito)
                return View("Agradecimento", registroFeito);
            }
            else
            {   // tendo erro retorna para pagina principal Index com os erros
                return RedirectToAction("Index");
            }

        }


        // método post que faz o login do usuário
        [HttpPost]

        public async Task<ActionResult<UsuarioModel>> Login(LoginDto loginDto)
        {
            // esta tudo valido no modelo?
            if (ModelState.IsValid)
            {
                // se estiver acessa o metodo login usando o parametro passado no método
                var usuario = await _usuarioInterface.Login(loginDto);

                // não achou usuário
                if (usuario.Id == 0)
                {
                    // msg de erro
                    TempData["MensagemErro"] = "Credenciais inválidas!";
                    
                    // retorna a view de home com os dados de logindto
                    return View(loginDto);
                }
                else
                {
                    // existe o ususário manda msg de sucesso
                    TempData["MensagemSucesso"] = "Usuário logado com sucesso!";

                    // criando a sessão atravez do método CriarSessao usado os dados usuario
                    _sessaoInterface.CriarSessao(usuario);

                    // redireciona a pagina  Index da view de  Email
                    return RedirectToAction("Index", "Email");
                }

            }
            else
            {
                // se não estiver retorna a view de Home com os dados de logindto
                return View(loginDto);
            }


        }
    }
}
