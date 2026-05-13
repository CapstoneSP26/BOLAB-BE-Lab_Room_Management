using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.TabletAccounts.Common
{
    public class LoginTabletReturn
    {
        public string? Token { get; set; }
        public string ReturnUrl { get; set; }
    }
}
