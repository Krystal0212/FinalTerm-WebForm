using System.Web.Mvc;
using System.Web.Routing;

namespace WebForm
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Cart",
            //    url: "carts/buy",
            //    defaults: new { controller = "Carts", action = "Buy", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Cart",
                url: "Carts/Buy",
                defaults: new { controller = "Carts", action = "Buy", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Index",
                url: "orderLists/Index/{resellerID}",
                defaults: new { controller = "orderLists", action = "Index", resellerID = UrlParameter.Optional }
            );

        }
    }
}
