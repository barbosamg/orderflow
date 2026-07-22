using OrderFlow.Application.Products.Contracts;
using OrderFlow.Application.Products.Services;
using OrderFlow.Application.Customers.Contracts;
using OrderFlow.Application.Customers.Services;
using OrderFlow.Infrastructure;
using OrderFlow.Application.Orders.Contracts;
using OrderFlow.Application.Orders.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();