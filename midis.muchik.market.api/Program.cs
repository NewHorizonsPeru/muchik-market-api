using Microsoft.AspNetCore.Mvc;
using midis.muchik.market.api.IoC;
using midis.muchik.market.api.Middlewares;
using midis.muchik.market.api.Validations;
using midis.muchik.market.application.dto.security;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddMappgins();
builder.Services.AddDbContexts(builder.Configuration);
builder.Services.AddDependencyContainer();
builder.Services.AddModelFilter();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<AuthorizationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
