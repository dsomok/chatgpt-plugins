using ChatGPT.Plugins.Github.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterDependencies(builder.Configuration);

var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI(config =>
   {
       config.SwaggerEndpoint("v1/swagger.yaml", "Github Plugin");
   })
   .UseStaticFiles();

app.MapEndpoints();

await app.RunAsync();
