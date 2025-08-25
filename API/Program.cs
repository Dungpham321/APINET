using GDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using GCommon;
using API;
using API.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
          builder =>
          {
              builder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
          });
});

builder.Services.AddDbContext<GDBContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("GDatabase"), sqlOptionsAction => sqlOptionsAction.CommandTimeout(600));
});

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = long.MaxValue;
});

builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy(TokenAuthOption.TokenType, new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters.IssuerSigningKey = TokenAuthOption.Key;
        options.TokenValidationParameters.ValidAudience = TokenAuthOption.Audience;
        options.TokenValidationParameters.ValidIssuer = TokenAuthOption.Issuer;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ClockSkew = TokenAuthOption.ExpiresSpan;
    });
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TokenAuthOption.IdleTimeout;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseCors("AllowAllHeaders");

app.UseStaticFiles();

app.UseSession();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

        if (error != null && error is SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new RequestResult
            {
                State = RequestState.NotAuth,
                Msg = Resource.ExpiredToken,
            }));
        }
        else if (error != null && error.Error != null)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new RequestResult
            {
                State = RequestState.Failed,
                Msg = error.Error.Message,
            }));
        }
        else await next();
    });
});

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
