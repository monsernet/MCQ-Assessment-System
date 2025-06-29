using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Data;
using MCQInterviews.Repositories;
using MCQInterviews.Repositories.Admin;
using MCQInterviews.Repositories.DifficultyAllocations;
using MCQInterviews.Repositories.DifficultyTypes;
using MCQInterviews.Repositories.ExcelImport;
using MCQInterviews.Repositories.JobLevels;
using MCQInterviews.Repositories.JobTitles;
using MCQInterviews.Repositories.McqQuestions;
using MCQInterviews.Repositories.MCQs;
using MCQInterviews.Repositories.MCQTestResults;
using MCQInterviews.Repositories.Options;
using MCQInterviews.Repositories.Questions;
using MCQInterviews.Repositories.QuestionTypes;
using MCQInterviews.Repositories.ResponseTypes;
using MCQInterviews.Repositories.Themes;
using MCQInterviews.Repositories.Users;
using MCQInterviews.Repositories.VoiceSamples;
using MCQInterviews.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicaationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicaationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

//**** Implement Repositories
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IJobTitleRepository, JobTitleRepository>();
builder.Services.AddScoped<IJobLevelRepository, JobLevelRepository>();
builder.Services.AddScoped<IMCQRepository, MCQRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IOptionRepository, OptionRepository>();
builder.Services.AddScoped<IMcqQuestionRepository, McqQuestionRepository>();
builder.Services.AddScoped<IMCQTestResultRepository, MCQTestResultRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IQuestionExcelImport, QuestionExcelImport>();
builder.Services.AddScoped<IQuestionDifficultyTypeRepository, QuestionDifficultyTypeRepository>();
builder.Services.AddScoped<IMcqDifficultyTypeRepository, McqDifficultyTypeRepository>();
builder.Services.AddScoped<IDifficultyAllocationRepository, DifficultyAllocationRepository>();
builder.Services.AddScoped<IVoiceSampleRepository, VoiceSampleRepository>();
builder.Services.AddScoped<IMCQQuestionTypeRepository, MCQQuestionTypeRepository>();
builder.Services.AddScoped<IQuestionTypeRepository, QuestionTypeRepository>();
builder.Services.AddScoped<IResponseTypeRepository, ResponseTypeRepository>();

//**** Implement Servivces
builder.Services.AddScoped<IVoiceSampleService, VoiceSampleService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireAdminOrEditorRole", policy => policy.RequireRole("Admin", "Editor"));
    options.AddPolicy("NonAdminRole", policy =>
    {
        policy.RequireRole("Editor");
        policy.RequireRole("User");
    });
});




// Add services to the container.
builder.Services.AddControllersWithViews();


// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//**** Add Razor Pages to the container
builder.Services.AddRazorPages();

//**** Customize password requirements
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//**** Handling the populated dropdown list values in McqController (Ajax call)
app.MapControllerRoute(
    name: "mcq",
    pattern: "{controller=Mcq}/{action=GetJobTitles}/{themeId?}");



//**** Add Route for Identity Razor Pages
app.MapRazorPages();

// Create a scope to create the admin and add Role
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}



app.Run();
