using MVVM.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace MVVM.Model
{
    public class Customer : IDataErrorInfo
    {
        #region Creation
        public static Customer CreateNewCustomer()
        {
            return new Customer();
        }
        public static Customer CreateCustomer(
            double totalSales,
            string firstName,
            string lastName,
            bool isCompany,
            string email)
        {
            return new Customer
            {
                TotalSales = totalSales,
                FirstName = firstName,
                LastName = lastName,
                IsCompany = isCompany,
                Email = email
            };
        }
        protected Customer() { }
        #endregion // Creation

        #region State Properties
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool IsCompany { get; set; }
        public string LastName { get; set; }
        public double TotalSales { get; private set; }
        #endregion // State Properties

        #region IDataErrorInfo Members
        string IDataErrorInfo.Error { get { return null; } }
        string IDataErrorInfo.this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }
        #endregion // IDataErrorInfo Members


        #region Validation
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null)
                        return false;
                return true;
            }
        }
        static readonly string[] ValidatedProperties =
        {
            "Email",
            "FirstName",
            "LastName",
        };
        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;
            string error = null;
            switch (propertyName)
            {
                case "Email":
                    error = this.ValidateEmail();
                    break;

                case "FirstName":
                    error = this.ValidateFirstName();
                    break;

                case "LastName":
                    error = this.ValidateLastName();
                    break;

                default:
                    Debug.Fail("Unexpected property being validated on Customer: " + propertyName);
                    break;
            }
            return error;
        }
        string ValidateEmail()
        {
            if (IsStringMissing(this.Email))
            {
                return Resources.Customer_Error_MissingEmail;
            }
            else if (!IsValidEmailAddress(this.Email))
            {
                return Resources.Customer_Error_InvalidEmail;
            }
            return null;
        }
        string ValidateFirstName()
        {
            if (IsStringMissing(this.FirstName))
            {
                return Resources.Customer_Error_MissingFirstName;
            }
            return null;
        }
        string ValidateLastName()
        {
            if (this.IsCompany)
            {
                if (!IsStringMissing(this.LastName))
                    return Resources.Customer_Error_CompanyHasNoLastName;
            }
            else
            {
                if (IsStringMissing(this.LastName))
                    return Resources.Customer_Error_MissingLastName;
            }
            return null;
        }
        static bool IsStringMissing(string value)
        {
            return String.IsNullOrEmpty(value) || value.Trim() == String.Empty;
        }
        static bool IsValidEmailAddress(string email)
        {
            if (IsStringMissing(email))
                return false;
            // This regex pattern came from: http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        #endregion // Validation
    }
}