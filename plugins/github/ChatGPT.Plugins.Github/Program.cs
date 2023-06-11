using ChatGPT.Plugins.Github.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterDependencies(builder.Configuration);

var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI(config =>
   {
       config.SwaggerEndpoint("v1/swagger.yaml", "AskTheCode Plugin");
   })
   .UseStaticFiles();

app.MapControllers();

await app.RunAsync();
