using Microsoft.OpenApi.Models;
using Api.Services.DependentsServices;
using Api.Services.EmployeesServices;
using Api.Services.PaycheckServices.Validator;
using Api.Services.PaycheckServices;
using Api.Services.PaycheckServices.Calculator.Models;

namespace Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Register application services (mock implementations)
        builder.Services.AddSingleton<IEmployeeValidator, SinglePartnerValidator>(); // real implementation would have to set up validator based on employee home/work country laws
        builder.Services.AddSingleton<ICalculatorModel, BiWeeklyCalculatorModel>(); // real implementation would likely be per-tenant granularity
        builder.Services.AddSingleton<IDependentsService, DependentsService>();
        builder.Services.AddSingleton<IEmployeesService, EmployeesService>();
        builder.Services.AddSingleton<IPaycheckService, PaycheckService>();

        // Register AutoMapper so we don't have to manually map entities to DTOs
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Employee Benefit Cost Calculation Api",
                Description = "Api to support employee benefit cost calculations"
            });
        });
        

        var allowLocalhost = "allow localhost";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(allowLocalhost,
                policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(allowLocalhost);

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}