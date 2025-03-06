using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Interfaces;
using Repositories.Repository;
using Services.Interfaces;
using Services.Mapper;
using Services.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




//Email Service

// Register Repositories
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IKoiPondRepo, KoiPondRepo>();
builder.Services.AddScoped<IShapeRepo, ShapeRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IKoiVarietyRepo, KoiVarietyRepo>();
builder.Services.AddScoped<IMasterRepo, MasterRepo>();
builder.Services.AddScoped<IBookingOnlineRepo, BookingOnlineRepo>();

// Register Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IKoiVarietyService, KoiVarietyService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IKoiPondService, KoiPondService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IBookingOnlineService, BookingOnlineService>();


//Register Mapper
builder.Services.AddAutoMapper(typeof(AccountMappingProfile));
builder.Services.AddAutoMapper(typeof(KoiPondMappingProfile));
builder.Services.AddAutoMapper(typeof(MasterMappingProfile));
builder.Services.AddAutoMapper(typeof(BookingOnlineMappingProfile));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Swagger Configuration with JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KoiFengShuiConsultingSystem", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "Cookies";
})
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
}).AddCookie("Cookies")
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoiFengShuiConsultingSystem v1");
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
// Use CORS before other middleware
app.UseCors("AllowAllOrigins");
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
