global using SharedOfficeBooking.Domain.Entities;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedOfficeBooking;
using SharedOfficeBooking.Domain.Entities;
using SharedOfficeBooking.Infrastructure.Helpers;
using SharedOfficeBooking.Application;
using SharedOfficeBooking.Infrastructure.Repositories;
using SharedOfficeBooking.Infrastructure.Repositories.Booking;
using SharedOfficeBooking.Infrastructure.Repositories.Workspace;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddDbContext<SharedOfficeBookingDbContext>(opt =>
//opt.UseInMemoryDatabase("FoodDatabase"));
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SharedOfficeBooking.Infrastructure")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<SharedOfficeBookingDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
// builder.Services.AddCustomCors("AllowAllOrigins");
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

var jwtSettings = builder.Configuration.GetSection("JWT").Get<Configuration.JwtSettings>();
builder.Services.Configure<Configuration.JwtSettings>(builder.Configuration.GetSection("JWT"));
var key = Encoding.ASCII.GetBytes(jwtSettings.Secret); 
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
    })
    .AddJwtBearer(options =>
    {
    
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,      
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidAudience = jwtSettings.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.IncludeErrorDetails = true;

    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // 👈 use your frontend origin here
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // 👈 allow credentials
    });
});


builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("DevCorsPolicy");
// app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();