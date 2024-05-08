using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using App.Services;
using System.Numerics;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using App.Data;

namespace App 
{
    public class Startup 
    {
        public static string ContentRootPath {get; set;} = string.Empty;

        public Startup (IConfiguration configuration, IWebHostEnvironment env) 
        {
            Configuration = configuration;
            ContentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddDbContext<AppDbContext>(options => {
                string connectString = Configuration.GetConnectionString("AppMvcConnectionString");
                options.UseSqlServer(connectString);
            });  

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.Configure<RazorViewEngineOptions>(options => {
                //{0} --> ten Action
                //{1} --> ten Controller
                //{2} --> ten Area
                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
            });

            services.AddSingleton<ProductService>();

             services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

                    services.Configure<IdentityOptions> (options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (1); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất
            

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true; 
                
            });      

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });  

            // services.AddAuthentication()
            //         .AddGoogle(options => {
            //             var gconfig = Configuration.GetSection("Authentication:Google");
            //             options.ClientId = gconfig["ClientId"];
            //             options.ClientSecret = gconfig["ClientSecret"];
            //             // https://localhost:5001/signin-google
            //             options.CallbackPath =  "/dang-nhap-tu-google";
            //         })
            //         .AddFacebook(options => {
            //             var fconfig = Configuration.GetSection("Authentication:Facebook");
            //             options.AppId  = fconfig["AppId"];
            //             options.AppSecret = fconfig["AppSecret"];
            //             options.CallbackPath =  "/dang-nhap-tu-facebook";
            //         });

            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            // User thoa 2 dieu kien thi moi co menu manage
            services.AddAuthorization(options => {
                options.AddPolicy("ViewManageMenu", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole(RoleName.Administrator);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // Thông báo chỉ truy cập bằng https
                app.UseHsts();
            }

            // Middleware HttpsRedirection -> Chuyển hướng http sang https
            app.UseHttpsRedirection ();
 
            // StaticFileMiddleware - truy cập file tĩnh
            app.UseStaticFiles ();

            app.UseStatusCodePages();

            app.UseRouting ();

            app.UseAuthentication(); //Xac dinh danh tinh
            app.UseAuthorization(); //Xac dinh quyen truy cap

            // Tạo các Endpoint
            app.UseEndpoints (endpoints => {

                endpoints.MapAreaControllerRoute(
                    name: "product",
                    pattern: "/{controller}/{action=Index}/{id?}",
                    areaName: "ProductManage"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:"/{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}