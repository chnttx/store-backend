using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using WebApplication2.Data;
using WebApplication2.Jobs;
using WebApplication2.Services;
using WebApplication2.Services.Implementation;
using WebApplication2.Services.Interface;

namespace WebApplication2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });

            string? connectionString = Configuration["ConnectionStrings:WebApiDatabase"] ?? string.Empty;
            services.AddDbContext<DataContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddTransient<IDataValidator, DataValidator>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("Accept-Version"),
                    new MediaTypeApiVersionReader("api-version"));
            });

            services.AddQuartz(opt =>
            {
                // opt.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey("UpdateVipJob");
                opt.AddJob<UpdateVipJob>(options => options.WithIdentity(jobKey));
                opt.AddTrigger(options =>
                {
                    options.ForJob(jobKey)
                        .WithIdentity("UpdateVipJob-trigger")
                        .WithCronSchedule(Configuration.GetSection("UpdateVipJob:CronSchedule")
                            .Value ?? "0 * * * * ?");
                });
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.OAuthClientId("secret_key");
                c.OAuthAppName("My API");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
