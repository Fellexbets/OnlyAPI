using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Igor_AIS_Proj.Auxiliary
{
    public class PasswordHasher
    {
        public static byte[] GenerateSalt()
        {
            //Método gera um salt aleatório para cada registo de utilizador
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }

        public static byte[] ReturnSalt(string salt)
        {
            //Recebe a string salt da BD e converte para byte[] para fazer novo hash (para comparar hashes)
            byte[] saltConverted = Convert.FromBase64String(salt);
            return saltConverted;
        }

        public static (string, string) HashPassword(string password, byte[] salt)
        {
            byte[] hashbytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8);

            //Conversão de bytes para strings para gravar na BD
            string hashedPass = Convert.ToBase64String(hashbytes);
            string saltConverted = Convert.ToBase64String(salt);

            return (hashedPass, saltConverted);
        }

        public static (string, string) ReturnHashedPasswordAndSalt(string password)
        {
            //Método gera um salt aleatório para cada registo de utilizador
            byte[] salt = GenerateSalt();
            //Devolve um hash da password com o salt gerado para gravar na BD
            (string passwordHashed, string saltConverted) = HashPassword(password, salt);

            return (passwordHashed, saltConverted);
        }

        //Método faz hash à password inserida com o salt guardado na BD e compara os hashes
        public static bool CompareHashedPasswords(string enteredPassword, string storedPassword, string passwordSalt)
        {
            byte[] salt = ReturnSalt(passwordSalt);
            (string enteredPasswordHashed, string saltConverted) = HashPassword(enteredPassword, salt);

            if (enteredPasswordHashed == storedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
