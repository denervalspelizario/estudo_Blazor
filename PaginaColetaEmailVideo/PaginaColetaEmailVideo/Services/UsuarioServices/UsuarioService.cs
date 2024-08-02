using Microsoft.EntityFrameworkCore;
using PaginaColetaEmailVideo.Data;
using PaginaColetaEmailVideo.Dto;
using PaginaColetaEmailVideo.Models;
using System.Security.Cryptography;

namespace PaginaColetaEmailVideo.Services.UsuarioServices
{
    public class UsuarioService: IUsuarioInterface
    {
        // acessando o banco
        private readonly AppDbContext _context;
        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        // método que cadastra usuário
        public async Task<UsuarioModel> Cadastrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            try
            {
                // cria senha hash e salt a partir  senha do usuario,
                // esse método devolve(out) a senhaHash e senhaSalt
                CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                // criando um usuario com base no UsuarioModel e as infos passadas
                var usuario = new UsuarioModel()
                {
                    Usuario = usuarioCriacaoDto.Usuario,
                    Email = usuarioCriacaoDto.Email,
                    SenhaHash = senhaHash, // métodoCriarSenhaHash criou as duas senhas(salt e hash)
                    SenhaSalt = senhaSalt
                };

                // adicionando e salvando no banco
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        // metodo usado no método Cadastrar para criar senhaHash e senhaSalt
        public void CriarSenhaHash(string senha, out byte[] senhaHash, out byte[] senhaSalt)
        {
            // criando a criptografia 512
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                // senha salt essa senha salt permite que a senha seja unica mesmo que
                // 2 usuarios criem a senha baseado na mesma senhaHash
                senhaSalt = hmac.Key;  

                // senhaHash baseado na senha criada pelo usuario
                senhaHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
            }
        }

        // método que faz o login so usuario
        public async Task<UsuarioModel> Login(LoginDto loginDto)
        {
            try
            {
                // verifica se email existe
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(user => user.Email == loginDto.Email);

                // se não existir
                if (usuario == null)
                {
                    // retorna um usuariovazio pois criamos a regra que se tiver um usuario vazio
                    // retorna msg de credenciais inválidas
                    return new UsuarioModel();
                }


                // usando o método que verifa senha ele retorna um true ou false
                if (!VerificarSenha(loginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    // se retorna false então as senhas não batem logo
                    // retorna um usuariovazio pois criamos a regra que se tiver um usuario vazio
                    // retorna msg de credenciais inválidas
                    return new UsuarioModel();
                }

                // se funcionar significa que as senhas batem então
                return usuario;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        // método que verifica a senha que será usado no método Login retorna um bool
        public bool VerificarSenha(string senha, byte[] senhaHash, byte[] senhaSalt)
        {
            /* qual é o fluxo?
               vamos criptografar usando a senha e comparar com a senhaHash que está no banco
               mas essa criptografia vai ser baseada na senha salt */

            // criando a ciptografia da senha passada baseada na senhaSalt
            using (HMACSHA512 hmac = new HMACSHA512(senhaSalt))
            {
                // criando criptografia baseado na senha e na senhaSalt
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                
                /* retornando um bool(true ou false) se senha que foi criptografada é igual
                   a senhaHash que já esta no banco se for true então as senhas são iguais 
                   se retornar false as senhas são diferentes */
                return computedHash.SequenceEqual(senhaHash);
            }
        }
    }
}
