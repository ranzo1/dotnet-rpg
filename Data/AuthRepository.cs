using System.Threading.Tasks;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
            if(user == null){
                response.Success = false;
                //poruka user not found ne bi trebala da se koristi, jer zlonamernom hakeru moze da posluzi prilikom hakovanja sajta
                response.Message = "User not found.";
            }
            else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else{
                response.Data = user.Id.ToString();
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {   
            ServiceResponse<int> response = new ServiceResponse<int>();
            if(await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if(await context.Users.AnyAsync(x => x.Username.ToLower().Equals(username.ToLower())))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //Hashovanje korisnicke sifre se sastoji iz kombinacije Hash i Salt vrednosti, koje prolaze kroz HMACSHA512 algoritam
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i = 0; i < computedHash.Length; i++){
                    if(computedHash[i] != passwordHash[i]){
                        return false;
                    }
                }
                return true;
            }
        }
    }
}