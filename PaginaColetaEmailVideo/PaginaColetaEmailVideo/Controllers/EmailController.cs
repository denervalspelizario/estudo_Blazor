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

        // injeções de dependencia acessando os métodos do EmailService para realizar as requisições
        private readonly IEmailInterface _emailInterface;

        public EmailController(IEmailInterface emailInterface)
        {
            _emailInterface = emailInterface;

        }

        /* metodo Index que retorna pagina Index de Email com toda a Lista(List) de EmailModels
           passado a pesquisa no parametro ou não  */
        public async Task<ActionResult<List<EmailModel>>> Index(string? pesquisar)
        {
            // não passou algum parametro no pesquisar então retorna pagina view de Email com  os
            // com a buscar  dos emails ou nomes baseado no string pesquisar
            if (pesquisar != null)
            {
                var resgistrosEmailsFiltro = await _emailInterface.ListarEmails(pesquisar);
               
                // retorna a View do Email com filtro de pesquisar
                return View(resgistrosEmailsFiltro);
            }


            /* não passou nenhum parametro em pesquisar então chamará método que 
               lista TODOS os emails*/
            var registrosEmails = await _emailInterface.ListarEmails();
            
            // retorna a View com a lista de email registrados
            return View(registrosEmails);
        }


        // metodo get que busca email por id e mostra em detalhes
        [HttpGet]
        public async Task<ActionResult<EmailModel>> DetalhesEmail(int id)
        {
            
            // buscando email por id usando o método ListarEmailPorId
            var registroEmail = await _emailInterface.ListarEmailPorId(id);
            
            //retorna a View DetalhesEmail com dados do email encontrado(registroEmail)
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
