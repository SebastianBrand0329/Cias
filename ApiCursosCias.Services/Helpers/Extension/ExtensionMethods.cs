using ApiCursosCias.Models.Entities;
using ApiCursosCias.Services.Helpers.Middleware;
using ApiCursosCias.Services.Helpers.Settings;
using FluentValidation.Results;
using IM.Encrypt.Access.Models;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ApiCursosCias.Services.Helpers.Extension;

public static class ExtensionMethods
{
    public static AppSettings Decryt(this AppSettings appSettings)
    {
        if (!appSettings.Decrypt)
        {
            appSettings.Connection = Encryt.Decrypt(appSettings.Connection, appSettings.Secret);

            if (appSettings.Auth != null)
            {
                appSettings.Auth.User = Encryt.Decrypt(appSettings.Auth.User, appSettings.Secret);
                appSettings.Auth.Pass = Encryt.Decrypt(appSettings.Auth.Pass, appSettings.Secret);
            }

            if (appSettings.TokenManagement != null)
                appSettings.TokenManagement.Secret = Encryt.Decrypt(appSettings.TokenManagement.Secret, appSettings.Secret);

            if (appSettings.EmailSettings != null)
            {
                appSettings.EmailSettings.From = Encryt.Decrypt(appSettings.EmailSettings.From, appSettings.Secret);
                appSettings.EmailSettings.Pass = Encryt.Decrypt(appSettings.EmailSettings.Pass, appSettings.Secret);
            }

            appSettings.Repository?.Folders?.ForEach(f =>
            {
                f.Name = f.File;
                f.File = @$"{appSettings.Repository.Directory}\{f.Path}";
                f.Path = @$"{appSettings.Repository.Url}/{f.Path}";
            });

            appSettings.Decrypt = true;
        }

        return appSettings;
    }

    public static Response<T> Warn<T, R>(this Response<T> response, object request = null, Response<R> result = null)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response));

        response.StatusMessage = result?.StatusMessage;

        DatabasePath();
        LoggerManager.logger.WithProperty("inputParams", request).Error(response.StatusMessage);

        return response;
    }

    public static Response<T> Warn<T>(this Response<T> response, object request = null)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response));

        DatabasePath();
        LoggerManager.logger.WithProperty("inputParams", request).Warn(response.StatusMessage);

        return response;
    }

    public static Response<T> Warn<T>(this Response<T> response, Exception ex, object request = null)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response));

        response.StatusMessage = $"{ex.Message} {ex.InnerException}";

        DatabasePath();
        LoggerManager.logger.WithProperty("inputParams", request).Warn(response.StatusMessage);

        return response;
    }

    public static Response<T> Error<T>(this Response<T> response, ResultException ex, object request = null)
    {
        response.StatusMessage = $"{ex.Detail} {ex.Message}";
        response.StatusCode = ex.Code;

        DatabasePath();
        LoggerManager.logger.WithProperty("inputParams", request)
            .Error(@$"{ex.Message}. Detail:{ex.Detail}. StackTrace: {ex.StackTrace} Source: {ex.Source}");

        return response;
    }

    public static Response<T> Exception<T>(this Response<T> response, Exception ex, object request = null)
    {
        response.StatusMessage = $"{ex.Message} {ex.InnerException}";

        DatabasePath();
        LoggerManager.logger.WithProperty("inputParams", request)
            .Error(@$"{ex.Message} {ex.InnerException} StackTrace: {ex.StackTrace} Source: {ex.Source}");

        return response;
    }

    public static Response<T> Success<T>(this Response<T> response, object request = null, Response<T> result = null)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response));

        response.Result = result != null ? result.Result : response.Result != null ? response.Result : default;
        response.StatusCode = AppSettings.Settings.ResponseResult;
        response.StatusMessage = "OK";
        response.HttpStatusCode = HttpStatusCode.OK;
        response.IsSuccess = true;

        var method = new StackTrace().GetFrame(1).GetMethod().GetRealMethodFromAsyncMethod()?.Name?.ToLower();

        var segment = AppSettings.Settings?.Logging?.Segments?.Where(s => s.ToLower().Equals(method))?.SingleOrDefault();
        if (segment != null)
        {
            DatabasePath();
            LoggerManager.logger.WithProperty("inputParams", request).WithProperty("response", response)
            .Info(response.StatusMessage);
        }
        return response;
    }

    public static ResponseProblem Bad<T>(this ResponseProblem error, Response<T> response)
    {
        error.StatusMessage = response.StatusMessage;
        error.StatusCode = response.StatusCode;
        return error;
    }

    public static ResponseProblem Bad(this ResponseProblem error)
    {
        DatabasePath();
        LoggerManager.logger.WithProperty("response", error.Title).Error(error.StatusMessage);
        return error;
    }

    public static ResponseProblem Error(this ResponseProblem error, ValidationResult response)
    {
        var errors = string.Join('|', response.Errors?.Select(v => v.ErrorMessage));

        error.Title = "One or more validation errors occurred";
        error.StatusCode = AppSettings.Settings.ErrorResult;
        error.StatusMessage = errors;
        return error;
    }

    public static MethodBase GetRealMethodFromAsyncMethod(this MethodBase asyncMethod)
    {
        var generatedType = asyncMethod.DeclaringType;
        var originalType = generatedType.DeclaringType;
        if (originalType != null)
        {
            var matchingMethods =
                from methodInfo in originalType.GetMethods()
                let attr = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                where attr != null && attr.StateMachineType == generatedType
                select methodInfo;

            // If this throws, the async method scanning failed.
            var foundMethod = matchingMethods.Single();
            return foundMethod;
        }
        else
            return asyncMethod;
    }

    private static void DatabasePath()
    {
        if (!string.IsNullOrEmpty(AppSettings.Settings.Logging.DbName))
        {
            var date = DateTime.Now;
            var dateFolder = @$"{date:yyyy}\{date.ToString("MMMM", CultureInfo.InvariantCulture)}\{date:dd}";
            string sourceDatabase = @$"{AppSettings.Settings.Logging.DefaultConnection}\{dateFolder}\{AppSettings.Settings.Logging.FolderDatabase}";
            if (!Directory.Exists(sourceDatabase))
                Directory.CreateDirectory(sourceDatabase);

            string fileCompact = @$"{sourceDatabase}\{AppSettings.Settings.Logging.DbName}";
            if (!File.Exists(fileCompact))
            {
                var connectionString = $@"Data Source={fileCompact}";
                var optionsBuilder = new DbContextOptionsBuilder<LogDbContext>();
                optionsBuilder.UseSqlite(connectionString);
                using var context = new LogDbContext(optionsBuilder.Options);
                context.Database.EnsureCreated();
            }
        }
    }

    public static void RemoveOldRefreshTokens(this UsuarioExtend user)
    {
        var appSettings = AppSettings.Settings.TokenManagement;
        // remove old inactive refresh tokens from user based on TTL in app settings
        user.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            DateTime.Parse(x.Created).AddDays(appSettings.RefreshExpiration) <= DateTime.Now);
    }

    //public static void RemoveOldRefreshTokens(this UsuarioMovilExtend user)
    //{
    //    var appSettings = AppSettings.Settings.TokenManagement;
    //    // remove old inactive refresh tokens from user based on TTL in app settings
    //    user.RefreshTokens.RemoveAll(x =>
    //        !x.IsActive &&
    //        DateTime.Parse(x.Created).AddDays(appSettings.RefreshExpiration) <= DateTime.Now);
    //}

    public static string HtmlTemplateToPDF(string stringHTML, string nameFolder, string folder = null)
    {
        var repository = AppSettings.Settings.Repository.Folders.Where(p => p.Name.Equals(nameFolder)).FirstOrDefault();

        var dateFolder = folder ?? $"{DateTime.Now.Ticks}";
        var _directory = $@"{repository.File}\{dateFolder}";
        var _url = $"{repository.Path}/{dateFolder}".Replace(@"\", "/");
        if (!Directory.Exists(_directory))
            Directory.CreateDirectory(_directory);

        string fileDestino = $@"{_directory}\{dateFolder}.pdf";

        // instantiate a html to pdf converter object
        HtmlToPdf converter = new();

        converter.Options.PdfPageSize = PdfPageSize.A4; // Tamaño de la página

        // create a new pdf document converting an url
        PdfDocument doc = converter.ConvertHtmlString(stringHTML);
        // save pdf document
        doc.Save(fileDestino);
        // close pdf document
        doc.Close();

        return fileDestino;
    }
}