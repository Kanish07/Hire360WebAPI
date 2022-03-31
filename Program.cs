using Hire360WebAPI.Models;
using Hire360WebAPI.Helpers;
using Hire360WebAPI.Services;
using Hire360WebAPI.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<Hire360Context>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc(
            "v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Hire360 API",
                Description = "Hire360 RESTful APIs",
                Contact = new OpenApiContact
                {
                    Name = "Hire360",
                    Email = "contacts.hire360@gmail.com",
                },
            });
        options.AddSecurityDefinition(
            "Bearer",
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });
        options.AddSecurityRequirement(
            new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new string[] { }
                },
            });
    });

builder.Services.AddCors();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<ICandidateServices, CandidateServices>();
builder.Services.AddScoped<IHumanResourceServices, HumanResourceServices>();
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        // Add the following line:
        webBuilder.UseSentry(o =>
        {
            o.Dsn = "https://a2424045248b4103b33195b4fa5746ec@o1181351.ingest.sentry.io/6294675";

            // When configuring for the first time, to see what the SDK is doing:
            o.Debug = true;

            // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
            // We recommend adjusting this value in production.
            o.TracesSampleRate = 1.0;
        });
    });

builder.WebHost.UseSentry();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSentryTracing();

app.UseMiddleware<JwtHelper>();

app.MapControllers();

app.Run();