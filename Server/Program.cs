
using Server.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IAuthService, AuthService>();
        //builder.Services.AddScoped<IListingService, ListingsService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyAllowSpecificOrigins",
                 builder =>
                 {
                     builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                 });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors("MyAllowSpecificOrigins");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
