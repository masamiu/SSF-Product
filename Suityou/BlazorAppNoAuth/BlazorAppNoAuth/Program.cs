using BlazorAppNoAuth.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Fast.Components.FluentUI;
using Suityou.Framework.Web.Service;

var builder = WebApplication.CreateBuilder(args);

// 多言語対応
builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// FluentUI
builder.Services.AddHttpClient();
builder.Services.AddFluentUIComponents();

// アプリケーション設定サービス
builder.Services.AddSingleton<AppSettingService>();

// ログインユーザを取得するために追加
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 多言語対応設定
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
