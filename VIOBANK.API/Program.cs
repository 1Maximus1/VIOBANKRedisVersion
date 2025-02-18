using VIOBANK.PostgresPersistence;
using Microsoft.EntityFrameworkCore;
using VIOBANK.Application.Services;
using VIOBANK.Application.Validation;
using VIOBANK.PostgresPersistence.Repositories;
using VIOBANK.Domain.Stores;
using FluentValidation;
using VIOBANK.Infrastructure;
using VIOBANK.API.Extensions;
using VIOBANK.API.Contracts.User;
using StackExchange.Redis;
using VIOBANK.RedisPersistence.Services;
using VIOBANK.API.Middleware;
using VIOBANK.API.Validation;
using VIOBANK.API.Contracts.Transaction;
using VIOBANK.API.Contracts.MobileTopup;
using VIOBANK.API.Contracts.Deposit;
using VIOBANK.API.Contracts.Contact;
using VIOBANK.API.Contracts.Card;
using VIOBANK.API.Contracts.Auth;
using VIOBANK.API.Validation.Auth;
using FluentValidation.AspNetCore;
using VIOBANK.API.Contracts.Account;
using VIOBANK.API.Validation.Account;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables();

builder.Configuration["ConnectionStrings:VIOBANKDbContext"] = Environment.GetEnvironmentVariable("VIOBANKDbContext");
builder.Configuration["ConnectionStrings:Redis"] = Environment.GetEnvironmentVariable("Redis");
builder.Configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JwtSecretKey");
builder.Configuration["ExchangeRateApi:ApiKey"] = Environment.GetEnvironmentVariable("ExchangeRateApiKey");
builder.Configuration["Twilio:AccountSid"] = Environment.GetEnvironmentVariable("TwilioAccountSid");
builder.Configuration["Twilio:AuthToken"] = Environment.GetEnvironmentVariable("TwilioAuthToken");
builder.Configuration["Encryption:SecretKey"] = Environment.GetEnvironmentVariable("EncryptionSecretKey");

var connectionString = builder.Configuration.GetConnectionString("VIOBANKDbContext");
builder.Services.AddDbContext<VIOBANKDbContext>(options =>
    options.UseNpgsql(connectionString));


// Connect API currency exchange
var exchangeRateApiUrl = builder.Configuration["ExchangeRateApi:BaseUrl"];
var exchangeRateApiKey = builder.Configuration["ExchangeRateApi:ApiKey"];

// Connect Twilio
var twilioAccountSid = builder.Configuration["Twilio:AccountSid"];
var twilioAuthToken = builder.Configuration["Twilio:AuthToken"];
var twilioPhoneNumber = builder.Configuration["Twilio:PhoneNumber"];

// Connect encryption
var encryptionSecretKey = builder.Configuration["Encryption:SecretKey"];

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddScoped<IUserStore, UsersRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IAccountStore, AccountsRepository>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddScoped<IContactStore, ContactsRepository>();
builder.Services.AddScoped<ContactService>();

builder.Services.AddScoped<ICardStore, CardsRepository>();
builder.Services.AddScoped<CardService>();

builder.Services.AddScoped<IDepositStore, DepositsRepository>();
builder.Services.AddScoped<DepositService>();

builder.Services.AddScoped<IMobileTopupStore, MobileTopupsRepository>();
builder.Services.AddScoped<MobileTopupService>();

builder.Services.AddScoped<ISettingsStore, SettingsRepository>();
builder.Services.AddScoped<SettingsService>();

builder.Services.AddScoped<ITransactionStore, TransactionsRepository>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddScoped<IValidator<UserProfileDTO>, UserProfileDTOValidator>();

builder.Services.AddScoped<IValidator<int>, UserIdValidator>();

builder.Services.AddScoped<IValidator<TransactionRequestDTO>, TransactionRequestValidator>();

builder.Services.AddScoped<IValidator<MobileTopupRequestDTO>, MobileTopupRequestValidator>();

builder.Services.AddScoped<IValidator<DepositRequestDTO>, DepositRequestDTOValidator>();

builder.Services.AddScoped<IValidator<DepositTopUpDTO>, DepositTopUpDTOValidator>();

builder.Services.AddScoped<IValidator<ContactRequestDTO>, ContactRequestDTOValidator>();

builder.Services.AddScoped<IValidator<CardDTO>, CardTypeValidator>();

builder.Services.AddScoped<IValidator<RegisterUserDTO>, RegisterUserDTOValidator>();

builder.Services.AddScoped<IValidator<LoginUserDTO>, LoginUserDTOValidator>();

builder.Services.AddScoped<IValidator<ChangeCardPasswordDTO>, ChangeCardPasswordDTOValidator>();

builder.Services.AddScoped<IValidator<ChangeAccountPassword>, ChangeAccountPasswordValidator>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddHttpClient<CurrencyExchangeService>(client =>
{
    client.BaseAddress = new Uri("https://api.exchangerate-api.com");
});

builder.Services.AddSingleton<SmsService>();

builder.Services.AddScoped<IDepositTransactionStore, DepositTransactionsRepository>();

builder.Services.AddScoped<IWithdrawnDepositStore, WithdrawnDepositRepository>();
builder.Services.AddScoped<WithdrawnDepositService>();

builder.Services.AddScoped<AesEncryptionService>();

//var redisConnection = builder.Configuration.GetConnectionString("Redis");
var redisConnection = builder.Configuration["ConnectionStrings:Redis"];

Console.WriteLine("Redis"+ " "+redisConnection);
//var redis = ConnectionMultiplexer.Connect(redisConnection);
//builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
//builder.Services.AddSingleton<JwtBlacklistService>();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddSingleton<JwtBlacklistService>();


builder.Services.AddApiAuthentication(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VIOBANKDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<JwtBlacklistMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});



app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
