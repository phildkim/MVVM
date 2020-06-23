using MVVM.ViewModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MVVM.Model
{
    // Credential Class validates user credentials
    public class Credential : ValidationViewModel, ICredential
    {
        #region Credential Fields
        private struct Data
        {
            internal string username;
            internal string password;
            internal string messages;
        }
        private Data _current;
        #endregion Credential Fields

        #region Credential Contructors
        protected Credential() { }
        public static Credential CreateNewCredential()
        {
            return new Credential();
        }
        public static Credential Create(string username, string password, string[] description)
        {
            return new Credential
            {
                Username = username,
                Password = password,
                Description = description,
            };
        }
        #endregion //Credential Contructors

        #region  Credential Properties Resources.AuthenticationService_UnauthorizedAccessException
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your username"), RegularExpression(@"^[a-zA-Z]{8,}$", ErrorMessage = "The username must contain at least 8 letters"), MaxLength(16, ErrorMessage = "The username must contain at most 16 letters")]
        public string Username
        {
            get { return _current.username; }
            set { SetProperty(ref _current.username, value); }
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter your password"), DataType(DataType.Password), RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Minimum eight characters, at least one letter and one number"), MaxLength(16, ErrorMessage = "The password must contain at most 16 characters")]
        public string Password
        {
            get { return _current.password; }
            set { SetProperty(ref _current.password, value); }
        }
        public string Status
        {
            get { return _current.messages; }
            set { SetProperty(ref _current.messages, value); }
        }
        public string[] Description { get; set; }
        #endregion // Credential Properties
    }

    // InternalCredential Class is stored in the database
    public class InternalCredential : ICredential
    {
        #region InternalCredential Contructors
        public static InternalCredential CreateNewCredential()
        {
            return new InternalCredential();
        }
        public static InternalCredential Create(string username, string password, string hashed, string[] description)
        {
            return new InternalCredential
            {
                Username = username,
                Password = password,
                Hashed = hashed,
                Description = description,
            };
        }
        protected InternalCredential() { }
        #endregion //InternalCredential Contructors

        #region  InternalCredential Properties
        [Key]
        public int UserId { get; private set; }
        public string Username { get; private set; }
        [Index(IsUnique = true)]
        public string Password { get; private set; }
        public string Hashed { get; private set; }
        public string[] Description { get; private set; }
        public virtual List<InternalCredential> InternalCredentials { get; private set; }
        #endregion // InternalCredential Properties
    }
}