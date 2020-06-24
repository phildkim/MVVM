using MVVM.Model;
using MVVM.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace MVVM.Service.Authentication
{
    public interface IAuthenticationService
    {
        Credential AuthenticateUser(string username, string password);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly List<InternalCredential> _users = new List<InternalCredential>()
        {
            InternalCredential.Create(
                    "phildkim", "Neg4life",
                    "m5p+O9L2Meykxj2KEL/WFbJ3/PxBKMwBUc8bGnT5sO0=",
                    new string[] { }),
            InternalCredential.Create(
                    "Mark", "Mark",
                    "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=",
                    new string[] { "Administrators" }),
            InternalCredential.Create(
                    "John", "John",
                    "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=",
                    new string[] { }),
        };
        public Credential AuthenticateUser(string username, string password)
        {
            InternalCredential userData = _users.FirstOrDefault(u => u.Username.Equals(username) && u.Hashed.Equals(CalculateHash(password, u.Username)));
            if (userData == null)
                throw new UnauthorizedAccessException(Resources.AuthenticationService_UnauthorizedAccess);
            return Credential.Create(userData.Username, userData.Password, userData.Description);
        }
        private string CalculateHash(string password, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(password + salt);
            HashAlgorithm algorithm = new SHA256Managed();

            // Return the hash as a base64 encoded string to be compared to the stored password
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            return Convert.ToBase64String(hash);
        }
    }
}