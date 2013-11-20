using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GXService.Authentication
{
    public class GXUserNameValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (userName != "show" || password != "test")
            {
                throw new FaultException("用户名或密码不正确，请重试");
            }
        }
    }
}
