using System;
using System.Collections.Generic;
using System.Text;
using DMXCore.DMXCore100.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DMXCore.DMXCore100
{
    internal class WebStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IIoManager ioManager)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/platforms", () => "Windows, Mac, Linux, Unix");

                endpoints.MapGet("/temperature", () =>
                {
                    var result = ioManager.DoThing();
                    if (result == null)
                        return "Unavailable";
                    else
                        return result.Value.DegreesCelsius.ToString("N2");
                });
            });
        }
    }
}
