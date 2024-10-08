using CurrencyXchange.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CurrencyXchange.Models;
using CurrencyXchange.Service;
using Microsoft.AspNetCore.Http.Features;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});
// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient<ICurrencyConverterService, CurrencyConverterService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();
builder.Services.AddScoped<AnalyticsService>(); // Register the AnalyticsService
builder.Services.AddScoped<PasswordHasher>();
 
string key = "aP!v@b0$1GxO1vH7uDkE2y4Q6r8tW9zT";
string iv = "16-char-init-vec";

builder.Services.AddSingleton(new EncyprtDecryptService(key, iv));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
	 options.TokenValidationParameters = new TokenValidationParameters
	 {
		 ValidateIssuer = true,
		 ValidateAudience = true,
		 ValidateLifetime = true,
		 ValidateIssuerSigningKey = true,
		 ValidIssuer = builder.Configuration["Jwt:Issuer"],
		 ValidAudience = builder.Configuration["Jwt:Audience"],
		 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	 };
 });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
