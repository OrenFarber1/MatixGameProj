using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatixBusinessLibrary
{
    interface MatixBuisnessInterface
    {
        bool AddPlayer(string firstName, string lastName, string nickName, string email, string passwordHash);


    }
}
