using Microsoft.EntityFrameworkCore;
using PaginaColetaEmailVideo.Data;
using PaginaColetaEmailVideo.Services.EmailServices;
using PaginaColetaEmailVideo.Services.SessaoServices;
using PaginaColetaEmailVideo.Services.UsuarioServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddHttpContextAccessor();


// indicando que as interfaces que implementam o metodos
builder.Services.AddScoped<IEmailInterface, EmailService>();
builder.Services.AddScoped<IUsuarioInterface, UsuarioService>();
builder.Services.AddScoped<ISessaoInterface, SessaoService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // [1]cuida com esse da erro


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//[1] faz parte da configuração HttpContextAcessor 
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
