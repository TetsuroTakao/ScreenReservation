using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using React.AspNet;
using System;
using System.IO;

namespace React.Sample.Webpack.CoreMvc
{
    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			#region insert settings value
			Environment.SetEnvironmentVariable("EncKey", Configuration["Encryptor:AesCryptoServiceProviderKey"]);
			Environment.SetEnvironmentVariable("EncIV", Configuration["Encryptor:AesCryptoServiceProviderIV"]);
			Environment.SetEnvironmentVariable("Redirect", Configuration["Redirect"]);
			Environment.SetEnvironmentVariable("RedirectLocal", Configuration["RedirectLocal"]);
			Environment.SetEnvironmentVariable("ChallengeSeed", Configuration["ChallengeSeed"]);
			Environment.SetEnvironmentVariable("ChallengeLength", Configuration["ChallengeLength"]);
			foreach (var s in Configuration.GetSection("SupportIdentities").GetChildren()) 
			{
				foreach (var s2 in s.GetChildren())
				{
					Environment.SetEnvironmentVariable(s.Key + "_" + s2.Key, s2.Value);
				}
			}
			#endregion
		}
		public IConfiguration Configuration { get; }
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(options=>options.EnableEndpointRouting = false);

			services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
				.AddChakraCore();
			services.AddReact();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			// Build the intermediate service provider then return it
			services.BuildServiceProvider();
		}
		public void Configure(IApplicationBuilder app, IHostEnvironment env)
		{
            #region // Initialise ReactJS.NET. Must be before static files.
            app.UseReact(config =>
			{
				config
					.SetReuseJavaScriptEngines(true)
					.SetLoadBabel(false)
					.SetLoadReact(false)
					.SetReactAppBuildPath("~/dist");
			});
			#endregion
			if (env.IsDevelopment())
			{
					app.UseDeveloperExceptionPage();
			}
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contents")),
				RequestPath = "/contents"
			});
			if (env.IsDevelopment())ServeFromDirectory(app, env.ContentRootPath, "node_modules");
			//app.UseRouting();
			//app.UseEndpoints(endpoints =>
			//         {
			//             endpoints.MapControllerRoute("default", "{path?}", new { controller = "Home", action = "Index" });
			//             endpoints.MapControllerRoute("comments-root", "comments", new { controller = "Home", action = "Index" });
			//             endpoints.MapControllerRoute("comments", "comments/page-{page}", new { controller = "Home", action = "Comments" });
			//         });
		}
		public void ServeFromDirectory(IApplicationBuilder app, string envContentRootPath, string path)
		{
			app.UseStaticFiles(new StaticFileOptions{FileProvider = new PhysicalFileProvider(Path.Combine(envContentRootPath, path)),RequestPath = "/" + path});
		}
	}
}
