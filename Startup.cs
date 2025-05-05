using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ListingFormWS.Data;
using Microsoft.OpenApi.Models;

namespace ListingFormWS
{
    /// <summary>
    /// Configures services and the application's request pipeline.
    /// </summary>
    public class Startup
    {
        private IConfiguration _configuration { get; }
        private IWebHostEnvironment _env;
        /// <summary>
        /// Gets the application's configuration settings.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class with the specified configuration and environment.
        /// </summary>
        /// <param name="configuration">The application's configuration settings.</param>
        /// <param name="env">The hosting environment information.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env=env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("secrets/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"secrets/appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <param name="services">The collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            string contentRootPath=_env.ContentRootPath;
            services.AddMvc(options => { options.EnableEndpointRouting=false;});
            services.Configure<Settings>(o=> { o.IConfigurationRoot = (IConfigurationRoot) Configuration; o.ContentPath=contentRootPath;} );
            //services.AddTransient<IListingFormRepository, ListingFormRepository>();
            //services.AddControllers().AddNewtonsoftJson();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Listing Form Web Service", Version = "v1" });
                var xmlPath = System.String.Format(@"{0}{1}ListingFormWS.xml",
                System.AppDomain.CurrentDomain.BaseDirectory, Path.DirectorySeparatorChar);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the HTTP request pipeline for the application.
        /// </summary>
        /// <param name="app">The application builder to configure the request pipeline.</param>
        /// <param name="env">The hosting environment information.</param>
        /// <param name="loggerFactory">The logger factory to configure logging.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions()
                {
                    SourceCodeLineCount=10
                };
                app.UseDeveloperExceptionPage(developerExceptionPageOptions);
                app.UseSwagger();
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }
             else
            {
                app.UseExceptionHandler("/Error");            
            }
            loggerFactory.AddLog4Net();  
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listing Form Web Service V1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
