using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
						 throw new InvalidOperationException("Connection string 'DefaultConnection' not found."
						 ));
});

builder.Services.AddAuthorization(); // As further "app.UseAuthorization();" is used later in the code, this line is necessary to ensure that authorization services are available.
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(options =>
{
	options.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader();
});

//Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "api");
	});
}

app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
