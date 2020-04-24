using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Request
{
    
    public class LoginRequestDTO
   {
        private string indexNumber;
        private string password;

        public LoginRequestDTO(string indexNumber, string password)
        {
            this.indexNumber = indexNumber;
            this.password = password;
        }

        public string Login { get; set; }
        public string Haslo { get; set; }
    }
}
