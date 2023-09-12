using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Filters;
using System;
using webapi.Context;
using webapi.DTOs.CustomerService;
using webapi.Helpers;
using webapi.Models;
using webapi.Repositories.DapperRepositories;
using webapi.Validator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using webapi.Infra;
using Microsoft.AspNetCore.Authentication;
using webapi.Hubs;
using webapi.Services;
using Hangfire;
using Hangfire.SqlServer;
using webapi.DTOs.Account;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddDbContext<BumpContext>(
		options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<BumpDapperContext>();
builder.Services.AddControllers();

builder.Services.AddScoped<PayService>();
builder.Services.AddScoped<HashUtility>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf(typeof(ProductFilterExample));

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// HttpClient
builder.Services.AddHttpClient("LinePay", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://sandbox-api-pay.line.me/");

    httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", builder.Configuration["LinePay:ChannelId"]);
});

// Helpers
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<HangfireHelper>();

// Repos
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<ShopRepository>();
builder.Services.AddScoped<OrderRepositiory>();
builder.Services.AddScoped<OnSaleRepository>();

// Validators
builder.Services.AddScoped<IValidator<ContactUsCreateDto>, ContactUsValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidation>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = new PathString("/");
    options.ExpireTimeSpan = TimeSpan.FromSeconds(5);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    options.ClaimActions.MapJsonKey("urn:google:email", "email", "string");
    options.ClaimActions.MapJsonKey("urn:google:name", "name", "string");
	options.SaveTokens = true;
})
.AddFacebook(facebookOptions =>
{
	facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
	facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/files"
});

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.MapHub<ChatHub>("/chat");

app.UseHangfireDashboard();

app.Run();
