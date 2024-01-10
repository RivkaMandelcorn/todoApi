using TodoApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        using (var dbContext = new ToDoDbContext())
        {
            var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAllOrigins",
//         builder =>
//         {
//             builder.AllowAnyOrigin()
//                 .AllowAnyMethod()
//                 .AllowAnyHeader();
//         });
// });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => {
                                    builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                    });
                    // builder.WithOrigins("http://localhost:3000"));
            });

            var app = builder.Build();

            app.UseCors("CorsPolicy");
            // app.UseCors("AllowAllOrigins");

            var items = dbContext.Items;

            app.MapGet("/items", () => items);

            app.MapGet("/items/{id}", (int id) =>
            {
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(item);
            });

            // POST new item
            app.MapPost("/items", (Item item) =>
            {
                dbContext.Add(item);
                dbContext.SaveChanges();
                return Results.Created($"/items/{item.Id}", item);
            });

            // DELETE item by id
            app.MapDelete("/items/{id}", (int id) =>
            {
                var item = items.FirstOrDefault(i => i.Id == id);
                if (item == null)
                {
                    return Results.NotFound();
                }
                dbContext.Remove(item);
                dbContext.SaveChanges();
                return Results.NoContent();
            });

            // PUT update item by id
            app.MapPut("/items/{id}", (Item updatedItem) =>
{
    var item = dbContext.Items.FirstOrDefault(i => i.Id == updatedItem.Id);
    if (item == null)
    {
        return Results.NotFound();
    }
    
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    
    dbContext.Update(item);
    dbContext.SaveChanges();
    
    return Results.Ok(item);
});


            app.Run();
        }
    }
}
