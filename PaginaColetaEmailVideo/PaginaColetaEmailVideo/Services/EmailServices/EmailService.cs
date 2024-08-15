using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaginaColetaEmailVideo.Data;
using PaginaColetaEmailVideo.Models;
using System.Net.Mail;
using System.Net;

namespace PaginaColetaEmailVideo.Services.EmailServices
{
    // Todas as operações referente ao email são feitas por IemailInterface e EmailService
    public class EmailService : IEmailInterface
    {

        // injeçõe de dependencia
        private readonly AppDbContext _context; // acessando banco
        private readonly IConfiguration _configuration; // email(appsettings SMTP)

        public EmailService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }


        // metodo que envia email(necessario configurar em appsettings o SMTP)
        public bool EnviarEmail(string email, string mensagem, string assunto)
        {
            try
            {
                // usar outlook pois precisa apena de 1 verificaçãoe  fica mais facil
                string host = _configuration.GetValue<string>("SMTP:Host");
                string nome = _configuration.GetValue<string>("SMTP:Nome");
                string username = _configuration.GetValue<string>("SMTP:Username");
                string senha = _configuration.GetValue<string>("SMTP:Senha");
                int porta = _configuration.GetValue<int>("SMTP:Porta");


                MailMessage mail = new MailMessage()
                {
                    // dados de quem está enviando esse email
                    From = new MailAddress(username, nome)
                };

                mail.To.Add(email);
                mail.Subject = assunto;
                mail.Body = mensagem;
                mail.IsBodyHtml = true; // tags htm para estilizar o email
                mail.Priority = MailPriority.High; // por ter maxima prioridade sera  enviado o mais rapido


                using (SmtpClient smtp = new SmtpClient(host, porta))
                {
                    smtp.Credentials = new NetworkCredential(username, senha); // credenciais do nosso email
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                   
                    return true; // deu certo enviou o email retorna um true
                }


            }
            catch (Exception ex)
            {
                return false; // deu ruim retorna false
            }

        }
        
        
        // método que lista email por id
        public async Task<EmailModel> ListarEmailPorId(int id)
        {
            try
            {

                // buscando registro que tenha mesmo id passado pelo parametro
                var registroEmail = await _context.Emails.FirstOrDefaultAsync(email => email.Id == id);

                // retorna o registro
                return registroEmail;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // método que lista emails por padrão se não colocar nenhum parametro será null
        public async Task<List<EmailModel>> ListarEmails(string pesquisar = null)
        {
            // resposta (que será uma lista de EmailModel)
            List<EmailModel> registrosEmails = new List<EmailModel>();
            
            try
            {
                // pesquisar é null? ou seja não passou nada
                if (pesquisar == null)
                {
                    // pega todos os registros de da tabela Emails no banco
                    registrosEmails = await _context.Emails.ToListAsync();
                }
                else
                {
                    /* se nao for, ou seja passou algum parametro então
                       usando o where busca na tabela Emails dados que são iguais 
                       ao pesquisar.Nome ou pesquisar.Email */ 
                    registrosEmails = await _context.Emails
                        .Where(email => email.Nome.Contains(pesquisar) ||
                               email.Email.Contains(pesquisar))
                        .ToListAsync();
                }

                // retorna a lista de email tendo filtro ou não
                return registrosEmails;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        // metodo que salva dados no banco
        public async Task<EmailModel> SalvarDadosCliente(EmailModel infoRecebida)
        {
            try
            {
                // adicionando dado no banco
                _context.Add(infoRecebida);

                // salvando dados no banco
                await _context.SaveChangesAsync();

                // resposta
                return infoRecebida;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
