using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;
using geographia.ags;
using SharePointInterface;
using ROWM.Dal;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Mvc;
using ROWM.Models;
using ROWM.Reports;
using ROWM.Dal.Repository;

namespace ROWM
{
    public class StartupSwanRelease1
    {
        public StartupSwanRelease1(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Add framework services.
            services.AddApplicationInsightsTelemetry();
            services.AddMvc();

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
            });

            var cs = Configuration.GetConnectionString("ROWM_Context");
            services.AddScoped<ROWM.Dal.ROWM_Context>(fac =>
            {
                return new Dal.ROWM_Context(cs);
            });

            services.AddScoped<ROWM.Dal.OwnerRepository>();
            services.AddScoped<ParcelStatusRepository>();
            services.AddScoped<ROWM.Dal.ContactInfoRepository>();
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddScoped<ROWM.Dal.AppRepository>();
            services.AddScoped<DeleteHelper>();
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new DocTypes(fac.GetRequiredService<ROWM_Context>()));
            services.AddScoped<Controllers.IParcelStatusHelper, Controllers.ParcelStatusHelperV2>();
            services.AddScoped<IUpdateParcelStatus,UpdateParcelStatus_wharton>();
            services.AddScoped<UpdateParcelStatus2>();

            var feat = new SwanLakeParcel("https://maps.hdrgateway.com/arcgis/rest/services/Oregon/SwanLanke_Parcel_FS/FeatureServer");
            services.AddSingleton<IFeatureUpdate>(feat);
            services.AddSingleton<IRenderer>(new SwanLakeParcel("https://maps.hdrgateway.com/arcgis/rest/services/Oregon/SwanLake_ROW_MapService/MapServer"));
            services.AddSingleton<IMapSymbology, SwanLakeSymbology>();

            services.AddSingleton<TxDotNeogitations.ITxDotNegotiation, TxDotNeogitations.Sh72>();

            services.AddScoped<ISharePointCRUD, DenverNoOp>();
            //var msi = new AzureServiceTokenProvider();
            //var vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(msi.KeyVaultTokenCallback));
            //var appid = vaultClient.GetSecretAsync("https://drippingsprings-keys.vault.azure.net/", "dev-id").GetAwaiter().GetResult();
            //var apps = vaultClient.GetSecretAsync("https://drippingsprings-keys.vault.azure.net/", "springs-secret").GetAwaiter().GetResult();
            //services.AddScoped<ISharePointCRUD, SharePointCRUD>(fac => new SharePointCRUD(
            //    __appId: appid.Value,
            //    __appSecret: apps.Value,
            //    _url: "https://hdroneview.sharepoint.com/sites/CoDS",
            //    subfolder: "Parcels",
            //    template: "Shared Documents/Parcels/_PARCEL_PARCEL No",
            //    d: fac.GetRequiredService<DocTypes>()));

            services.AddScoped<IRowmReports, WhartonReport>();

            services.AddSingleton<SiteDecoration, Swan>();

            services.AddSwaggerDocument();

            services.AddLogging(b => {
                b.AddConsole();
                b.AddDebug();
            });            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler("/Home/Error");
 
            app.UseStaticFiles();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
