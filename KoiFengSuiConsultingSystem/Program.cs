﻿using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.KoiPondRepository;
using Repositories.Repositories.KoiVarietyRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.MasterScheduleRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.RegisterAttendRepository;
using Repositories.Repositories.ShapeRepository;
using Repositories.Repositories.WorkShopRepository;
using Services.Mapper;
using Services.Services.AccountService;
using Services.Services.BookingService;
using Services.Services.CustomerService;
using Services.Services.EmailService;
using Services.Services.KoiPondService;
using Services.Services.KoiVarietyService;
using Services.Services.MasterService;
using Services.Services.MasterScheduleService;
using Services.Services.PaymentService;
using Services.Services.RegisterAttendService;
using Services.Services.WorkshopService;
using Services.ServicesHelpers.PriceService;
using System.Text;
using Repositories.Repositories.VarietyColorRepository;
using Repositories.Repositories.ColorRepository;
using Services.Services.CourseService;
using Repositories.Repositories.ChapterRepository;
using Services.Services.ChapterService;
using Services.Services.OrderService;
using Services.ServicesHelpers.BackGroundService;
using Repositories.Repositories.QuizRepository;
using Services.Services.QuizService;
using Repositories.Repositories.QuestionRepository;
using Services.Services.QuestionService;
using Repositories.Repositories.ConsultationPackageRepository;
using Services.Services.ConsultationPackageService;
using CloudinaryDotNet;
using BusinessObjects.Models;

using Services.ServicesHelpers.UploadService;
using System.Text.Json.Serialization;
using Repositories.Repositories.AnswerRepository;
using Repositories.Repositories.ContractRepository;
using Services.Services.ContractService;
using Services.Services.AnswerService;
using Repositories.Repositories.RegisterCourseRepository;
using Repositories.Repositories.EnrollChapterRepository;
using Services.Services.RegisterCourseService;
using Repositories.Repositories.EnrollQuizRepository;
using Repositories.Repositories.EnrollAnswerRepository;
using Repositories.Repositories.CategoryRepository;
using Services.Services.CategoryService;
using Services.ServicesHelpers.GoogleMeetService;
using Services.ServicesHelpers.RefundSerivce;
using Repositories.Repositories.FengShuiDocumentRepository;
using Services.Services.FengShuiDocumentService;
using Repositories.Repositories.AttachmentRepository;
using Services.Services.AttachmentService;
using Services.ServicesHelpers.BunnyCdnService;
using Repositories.Repositories.LocationRepository;
using Services.Services.LocationService;
using Services.ServicesHelpers.TimeOnlyJsonConverter;
using Services.Services.DashboardService;
using Repositories.Repositories.CertificateRepository;
using Repositories.Repositories.EnrollCertRepository;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;



var builder = WebApplication.CreateBuilder(args);

// Register Repositories
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IKoiPondRepo, KoiPondRepo>();
builder.Services.AddScoped<IShapeRepo, ShapeRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<IKoiVarietyRepo, KoiVarietyRepo>();
builder.Services.AddScoped<IVarietyColorRepo, VarietyColorRepo>();
builder.Services.AddScoped<IMasterRepo, MasterRepo>();
builder.Services.AddScoped<IBookingOnlineRepo, BookingOnlineRepo>();
builder.Services.AddScoped<IMasterScheduleRepo, MasterScheduleRepo>();
builder.Services.AddScoped<IBookingOfflineRepo, BookingOfflineRepo>();
builder.Services.AddScoped<ICourseRepo, CourseRepo>();
builder.Services.AddScoped<IWorkShopRepo, WorkShopRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IRegisterAttendRepo, RegisterAttendRepo>();
builder.Services.AddScoped<IColorRepo, ColorRepo>(); 
builder.Services.AddScoped<ICourseRepo, CourseRepo>();
builder.Services.AddScoped<IChapterRepo, ChapterRepo>();
builder.Services.AddScoped<IQuizRepo, QuizRepo>();
builder.Services.AddScoped<IQuestionRepo, QuestionRepo>();
builder.Services.AddScoped<IConsultationPackageRepo, ConsultationPackageRepo>();
builder.Services.AddScoped<IAnswerRepo, AnswerRepo>();
builder.Services.AddScoped<IContractRepo, ContractRepo>();
builder.Services.AddScoped<IRegisterCourseRepo, RegisterCourseRepo>();
builder.Services.AddScoped<IEnrollChapterRepo, EnrollChapterRepo>();
builder.Services.AddScoped<IEnrollQuizRepo, EnrollQuizRepo>();
builder.Services.AddScoped<IEnrollAnswerRepo, EnrollAnswerRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IFengShuiDocumentRepo, FengShuiDocumentRepo>();
builder.Services.AddScoped<IAttachmentRepo, AttachmentRepo>();
builder.Services.AddScoped<ILocationRepo, LocationRepo>();
builder.Services.AddScoped<ICertificateRepo, CertificateRepo>();
builder.Services.AddScoped<IEnrollCertRepo, EnrollCertRepo>();


// Register Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IKoiVarietyService, KoiVarietyService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IKoiPondService, KoiPondService>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IMasterScheduleService, MasterScheduleService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPriceService, PriceService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IWorkshopService, WorkshopService>();
builder.Services.AddScoped<IRegisterAttendService, RegisterAttendService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IConsultationPackageService, ConsultationPackageService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IRegisterCourseService, RegisterCourseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IFengShuiDocumentService, FengShuiDocumentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IBunnyCdnService, BunnyCdnService>();

// Register BackgroundService
builder.Services.AddHostedService<OrderExpirationBackgroundService>();
builder.Services.AddHostedService<BookingCleanupService>();

// Register GoogleMeetService
builder.Services.AddSingleton<GoogleMeetService>();

// BunnyCdn Configuration
//builder.Services.Configure<BunnyCdnSettings>(builder.Configuration.GetSection("BunnyCdn"));
// Register BunnyCdnService
//builder.Services.AddSingleton<IBunnyCdnService>(provider => {
//    var settings = provider.GetRequiredService<IOptions<BunnyCdnSettings>>().Value;
//    return new BunnyCdnService(settings);
//});

//Register Mapper
builder.Services.AddAutoMapper(typeof(AccountMappingProfile));
builder.Services.AddAutoMapper(typeof(KoiPondMappingProfile));
builder.Services.AddAutoMapper(typeof(MasterMappingProfile));
builder.Services.AddAutoMapper(typeof(BookingMappingProfile));
builder.Services.AddAutoMapper(typeof(MasterScheduleMappingProfile));
builder.Services.AddAutoMapper(typeof(BookingMappingProfile));
builder.Services.AddAutoMapper(typeof(RegisterAttendMappingProfile));
builder.Services.AddAutoMapper(typeof(WorkshopMappingProfile));
builder.Services.AddAutoMapper(typeof(CourseMappingProfile));
builder.Services.AddAutoMapper(typeof(ChapterMappingProfile));
builder.Services.AddAutoMapper(typeof(QuizMappingProfile));
builder.Services.AddAutoMapper(typeof(QuestionMappingProfile));
builder.Services.AddAutoMapper(typeof(ContractMappingProfile));
builder.Services.AddAutoMapper(typeof(ConsultationPackageMappingProfile));
builder.Services.AddAutoMapper(typeof(AnswerMappingProfile));
builder.Services.AddAutoMapper(typeof(FengShuiDocumentMappingProfile));
builder.Services.AddAutoMapper(typeof(AttachmentMappingProfile));
builder.Services.AddAutoMapper(typeof(EnrollChapterMappingProfile));
builder.Services.AddAutoMapper(typeof(LocationMappingProfile));
builder.Services.AddAutoMapper(typeof(CustomerMappingProfile));
builder.Services.AddAutoMapper(typeof(CertificateMappingProfile));


builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Add Response Caching Middleware
builder.Services.AddResponseCaching();

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

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 500_000_000; // 500MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 500_000_000; // 500MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500_000_000; // 500MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); 
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5261);
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
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.WithOrigins("https://koi-feng-sui-consulting-system-front-dq5ch5q4n.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    options.AddPolicy("AllowBunnyCDN",
        policy =>
        {
            policy.WithOrigins("https://vz-2fab5d8b-8fd.b-cdn.net/")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition", "Content-Length");
    });
    options.AddPolicy("AllowBackend", policy =>
    {
        policy.WithOrigins("http://koifengshui-001-site1.ltempurl.com/")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
// Register Cloudinary into Dependency Injection
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new CloudinaryDotNet.Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) 
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KoiFengShuiConsultingSystem v1");
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}
app.UseStaticFiles();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseCors("AllowAllOrigins");
app.UseCors("AllowVercel");
app.UseCors("AllowBunnyCDN");
app.UseCors("AllowBackend");
app.UseCors("DefaultCorsPolicy");
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
