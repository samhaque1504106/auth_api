using SignUp_Api.Data;
using Microsoft.EntityFrameworkCore;
using Bogus;
using SignUp_Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStrings"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //using (var scope = app.Services.CreateScope())
    //{
    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    SeedUserData(db); // Call the seed method here
    //}
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();

// Data Seeder Method
//static void SeedUserData(ApplicationDbContext db)
//{
    
//    var userFaker = new Faker<SignUp>()
//        .RuleFor(u => u.UserName, f => f.Internet.UserName())
//        .RuleFor(u => u.Password, f => f.Internet.Password(8))
//        .RuleFor(u => u.Email, f => f.Internet.Email())
//        .RuleFor(u => u.UserStatus, f => f.PickRandom(new[] { "Active", "Inactive" }))
//        .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset(1))
//        .RuleFor(u => u.UpdatedAt, f => f.Date.PastOffset(1))
//        .RuleFor(u => u.SignInStatus, f => f.PickRandom(new[] { "In", "Out" }));

//    var users = userFaker.Generate(10000); // Generate 10000 fake users

//    db.SignUp.AddRange(users);
//    db.SaveChanges();
//}
