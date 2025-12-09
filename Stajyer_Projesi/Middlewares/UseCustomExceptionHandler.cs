using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Application.Common; 

namespace API.Middlewares
{
    public static class UseCustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionFeature != null)
                    {
                        var ex = exceptionFeature.Error;
                        int statusCode = 500;

                        switch (ex)
                        {
                            case KeyNotFoundException: statusCode = 404; break;
                            case ArgumentException:
                            case InvalidOperationException: statusCode = 400; break;
                        }

                        context.Response.StatusCode = statusCode;

                        var response = Response<NoContent>.Fail(ex.Message, statusCode, true);

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}