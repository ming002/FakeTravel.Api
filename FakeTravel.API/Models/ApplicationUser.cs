using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Collections.Generic;

namespace FakeTravel.API.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        //名称必须保持一致
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}
