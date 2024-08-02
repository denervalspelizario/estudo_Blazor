using Microsoft.AspNetCore.Mvc;
using PaginaColetaEmailVideo.Filtros;
using PaginaColetaEmailVideo.Models;
using PaginaColetaEmailVideo.Services.EmailServices;

namespace PaginaColetaEmailVideo.Controllers
{
    // apenas se usuarios logados podem acessar os métodos e views de EmailController
    [UsuarioLogado]
    public class EmailController : Controller
    {

        // injeções de dependencia acessando os métodos para realizar as requisições
        private readonly IEmailInterface _emailInterface;

        public EmailController(IEmailInterface emailInterface)
        {
            _emailInterface = emailInterface;

        }

        // metodo que retorna Lista de EmailModels
        public async Task<ActionResult<List<EmailModel>>> Index(string? pesquisar)
        {
            if (pesquisar != null)
            {
                var resgistrosEmailsFiltro = await _emailInterface.ListarEmails(pesquisar);
               
                return View(resgistrosEmailsFiltro);
            }

            // se email existir chamará método que lista email
            var registrosEmails = await _emailInterface.ListarEmails();
            
            // retorna a View com a lista de email registrados
            return View(registrosEmails);
        }


        // metodo get que busca email por id e mostra em detalhes
        [HttpGet]
        public async Task<ActionResult<EmailModel>> DetalhesEmail(int id)
        {
            
            // buscando email por id
            var registroEmail = await _emailInterface.ListarEmailPorId(id);
            
            //retornando View com dados do email encontrado
            return View("DetalhesEmail",registroEmail);
        }

        // metodo post que envia um email
        [HttpPost]
        public async Task<ActionResult<EmailModel>> EnviarEmail(string enderecoEmail, string textoEmail, string assuntoEmail, int id)
        {
            // buscando email por id
            var email = await _emailInterface.ListarEmailPorId(id);

            // o status do email esta false
            if (email.Status == false)
            {
                // se estiver manda msg de erro e retorna a View DetalhesEmail
                TempData["MensagemErro"] = "Não é possível encaminhar email para um registro inativo!";
                return View("DetalhesEmail", email);
            }

            // dados que foram passados pelos parametros são nulos?
            if (textoEmail == null || assuntoEmail == null)
            {
                // msg de erro e retorna a view DetalhesEmail
                TempData["MensagemErro"] = "Insira um assunto e um corpo para o email!";
                return View("DetalhesEmail", email);
            }

            // chamando o método EnviarEmail que retorna um bool
            bool resultado = _emailInterface.EnviarEmail(enderecoEmail, textoEmail, assuntoEmail);

            // deu certo conseguiu enviar email?
            if (resultado == true)
            {
                // retorna mensagem de sucesso
                TempData["MensagemSucesso"] = "Email encaminhado com sucesso!";
            }
            else
            {
                // retorna mensagem de erro
                TempData["MensagemErro"] = "Ocorreu um problema no envio do email!";

            }

            // redireciona para Index
            return RedirectToAction("Index");
        }
    }



}
