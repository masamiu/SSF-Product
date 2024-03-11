using BlazorAppNoAuth.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Fast.Components.FluentUI;
using Suityou.Framework.Web.Service;

var builder = WebApplication.CreateBuilder(args);

// ������Ή�
builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// FluentUI
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

// �A�v���P�[�V�����ݒ�T�[�r�X
builder.Services.AddSingleton<AppSettingService>();

// ���O�C�����[�U���擾���邽�߂ɒǉ�
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ������Ή��ݒ�
var supportedCultures = new[] { "ja", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
