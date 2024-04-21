using ApiCentroMedicoClient.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//A�adimos
builder.Services.AddHttpContextAccessor();

//A�adimos dependencias
builder.Services.AddTransient<ServiceApiCentroMedicoClient>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

//A�adimos filtros
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

//Configuramos
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
//A�adimos session
app.UseSession();

//A�adimos para filtro
app.UseMvc(routes =>
{
    routes.MapRoute
    (
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
