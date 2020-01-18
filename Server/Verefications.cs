using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Verefications
    {
        private const string notAllowed = ",.!()";
        public static bool isCorrectLogin(string login)
        {
            if (login==null)
                return false;
            for (int i = 0; i < login.Length; i++)
                if (notAllowed.Contains(login[i]))
                    return false;
            if (login.Length < 5 || login.Length > 10)
                return false;
            return true;
        }
    }
}
