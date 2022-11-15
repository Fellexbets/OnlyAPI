
using Confluent.Kafka;
using Igor_AIS_Proj.Infrastructure.KafkaServices;
using Igor_AIS_Proj.MailServices;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json")
                   .Build();
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var producerConfig = new ProducerConfig();
configuration.Bind("producer", producerConfig);

builder.Services.AddSingleton<ProducerConfig>(producerConfig);

builder.Services.AddScoped<IAccountPersistence, AccountPersistence>();
builder.Services.AddScoped<IMovementPersistence, MovementPersistence>();
builder.Services.AddScoped<ITransferPersistence, TransferPersistence>();
builder.Services.AddScoped<IUserPersistence, UserPersistence>();
builder.Services.AddScoped<ISessionPersistence, SessionPersistence>();
builder.Services.AddScoped<IUploadResultPersistence, UploadResultPersistence>();
builder.Services.AddScoped<IAccountBusiness, AccountBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IMovementBusiness, MovementBusiness>();
builder.Services.AddScoped<ITransferBusiness, TransferBusiness>();
builder.Services.AddScoped<ISessionBusiness, SessionBusiness>();
builder.Services.AddScoped<IProducerHandler, ProducerHandler>();
builder.Services.AddTransient<IBasePersistence<User>, UserPersistence>();
builder.Services.AddTransient<IBasePersistence<Account>, AccountPersistence>();
builder.Services.AddTransient<IBasePersistence<Transfer>, TransferPersistence>();
builder.Services.AddTransient<IBasePersistence<Movement>, MovementPersistence>();
builder.Services.AddTransient<IBasePersistence<Session>, SessionPersistence>();
builder.Services.AddSingleton<IJwtServices, JwtServices>();
//builder.Services.AddSingleton<IHostedService, KafkaConsumerHandler>();

builder.Services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddSingleton<IHostedService, KafkaConsumerHandler>();
builder.Services.AddSingleton<IMailNotificationUseCase, MailNotificationUseCase>();
// logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});


builder.Services.AddAuthentication(configureOptions: x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.ASCII.GetBytes(configuration["Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseRouting();
app.UseDeveloperExceptionPage();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseAuthorization();

app.MapControllers();

app.Run();
