using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BookLAB.Domain.Common
{
    public class GoogleUser
    {
        public string NameIdentifier { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Email { get; set; }
    }
}
