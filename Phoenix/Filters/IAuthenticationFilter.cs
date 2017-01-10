using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Filters;

namespace Phoenix.Filters
{
    public interface IAuthenticationFilter
    {
        void OnAuthentication(AuthenticationContext filterContext);

        void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext);
    }
}
