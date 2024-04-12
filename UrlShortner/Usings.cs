global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Microsoft.Extensions.Caching.Memory;
global using Serilog;
global using ILogger = Serilog.ILogger;

global using UrlShortner.Common;
global using UrlShortner.DataAccess;
global using UrlShortner.Shorten;
global using UrlShortner.Settings;
global using UrlShortner.Shorten.Commands;
global using UrlShortner.Shorten.Queries;
