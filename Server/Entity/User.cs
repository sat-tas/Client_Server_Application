using System;
using System.Security.Cryptography;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Entity
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int Password { get; private set; }

        public string Salt { get; private set; }
        
        public string Email { get; set; }

        public User()
        {
        }

        public User(string name, string password, string email) : base()
        {

            Name = name ?? throw new ArgumentNullException(nameof(name));
            
            byte[] salt1 = new byte[32];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt1);
            }

            Salt=Encoding.Default.GetString(salt1);

            password += Salt;

            SHA384 sha1 = new SHA384CryptoServiceProvider();
            byte[] hashbytes = sha1.ComputeHash(Encoding.Default.GetBytes(password));
            Password = 0;
            for (int i = 0; i < hashbytes.Length; i++)
                Password += hashbytes[i];
               
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    
        public bool Authorization(string password)
        {
            password += this.Salt;
            SHA384 sha1 = new SHA384CryptoServiceProvider();

            int sum = 0;
            byte[] hashbytes = sha1.ComputeHash(Encoding.Default.GetBytes(password));
            for (int i = 0; i < hashbytes.Length; i++)
                sum += hashbytes[i];

            return this.Password==sum;
        }
        
        public void Change(string name, int password,string salt, string email)
        {
            this.Name = name;
            this.Password = password;
            this.Salt = salt;
            this.Email = email;
        }
    }
}