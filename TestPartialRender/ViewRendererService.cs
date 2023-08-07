using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TestPartialRender;

public class ViewRendererService
{
    private readonly IRazorViewEngine _razorViewEngine;

    private readonly ITempDataProvider _tempDataProvider;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewRenderer"/> class.
    /// </summary>
    public ViewRendererService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Renders a partial MVC view to string. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <returns>String of the rendered view or null on error</returns>
    public async Task<string> RenderView(string viewName, object? model = null)
    {
        return await RenderViewToString(viewName, model);
    }

    /// <summary>
    /// Renders a partial MVC view to the given writer. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="writer">Writer to render the view to</param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    public void RenderView(string viewName, TextWriter writer, object? model)
    {
        _ = RenderView(viewName, model, writer);
    }

    /// <summary>
    /// Renders a partial MVC view to string. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <param name="errorMessage">optional out parameter that captures an error message instead of throwing</param>
    /// <returns>String of the rendered view or null on error</returns>
    public string? RenderView(
        string viewName,
        object? model,
        out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            return RenderViewToString(viewName, model).Result;
        }
        catch (Exception ex)
        {
            errorMessage = ex.GetBaseException().Message;
        }

        return null;
    }

    /// <summary>
    /// Renders a partial MVC view to the given writer. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <param name="writer">Writer to render the view to</param>
    /// <param name="errorMessage">optional out parameter that captures an error message instead of throwing</param>
    public void RenderView(
        string viewName,
        object? model,
        TextWriter writer,
        out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            _ = RenderView(viewName, model, writer);
        }
        catch (Exception ex)
        {
            errorMessage = ex.GetBaseException().Message;
        }
    }

    /// <summary>
    /// Renders a partial MVC view to string. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <returns>String of the rendered view or null on error</returns>
    public async Task<string> RenderPartialView(string viewName, object? model = null)
    {
        return await RenderPartialViewToString(viewName, model);
    }

    /// <summary>
    /// Renders a partial MVC view to string. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="writer">Text writer to render view to</param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    public async Task RenderPartialView(
        string viewName,
        TextWriter writer,
        object? model = null)
    {
        await RenderPartialView(viewName, model, writer);
    }

    /// <summary>
    /// Renders a full MVC view to a string. Will render with the full MVC
    /// View engine including running _ViewStart and merging into _Layout
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to render the view with</param>
    /// <returns>String of the rendered view or null on error</returns>
    public async Task<string> RenderViewToString(string viewName, object? model = null)
    {
        return await RenderViewToStringInternal(viewName, model, false);
    }

    /// <summary>
    /// Renders a full MVC view to a writer. Will render with the full MVC
    /// View engine including running _ViewStart and merging into _Layout
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to render the view with</param>
    /// <param name="writer">writer used to write</param>
    public async Task RenderView(string viewName, object? model, TextWriter writer)
    {
        await RenderViewToWriterInternalAsync(viewName, writer, model, false);
    }

    /// <summary>
    /// Renders a partial MVC view to string. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <returns>String of the rendered view or null on error</returns>
    public async Task<string> RenderPartialViewToString(string viewName, object? model = null)
    {
        return await RenderViewToStringInternal(viewName, model, true);
    }

    /// <summary>
    /// Renders a partial MVC view to given Writer. Use this method to render
    /// a partial view that doesn't merge with _Layout and doesn't fire
    /// _ViewStart.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">The model to pass to the viewRenderer</param>
    /// <param name="writer">Writer to render the view to</param>
    public async Task RenderPartialView(string viewName, object? model, TextWriter writer)
    {
        await RenderViewToWriterInternalAsync(viewName, writer, model, true);
    }

    /// <summary>
    /// Internal method that handles rendering of either partial or
    /// or full views.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="writer">Text writer to render view to</param>
    /// <param name="model">Model to render the view with</param>
    /// <param name="partial">Determines whether to render a full or partial view</param>
    protected async Task RenderViewToWriterInternalAsync(string viewName, TextWriter writer, object? model = null, bool partial = false)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var viewEngineResult = _razorViewEngine.FindView(actionContext, viewName, !partial);

        if (viewEngineResult == null || viewEngineResult.View == null)
        {
            throw new FileNotFoundException();
        }

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var ctx = new ViewContext(
            actionContext,
            viewEngineResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions()
        );

        await viewEngineResult.View.RenderAsync(ctx);
    }

    /// <summary>
    /// Internal method that handles rendering of either partial or
    /// or full views.
    /// </summary>
    /// <param name="viewName">
    /// The name of the view including subfolders.
    /// </param>
    /// <param name="model">Model to render the view with</param>
    /// <param name="partial">Determines whether to render a full or partial view</param>
    /// <returns>String of the rendered view</returns>
    private async Task<string> RenderViewToStringInternal(string viewName, object? model, bool partial = false)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        using var sw = new StringWriter();
        var viewEngineResult = _razorViewEngine.FindView(actionContext, viewName, !partial);

        if (viewEngineResult == null || viewEngineResult.View == null)
        {
            throw new FileNotFoundException("Can't find view.");
        }

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var ctx = new ViewContext(
            actionContext,
            viewEngineResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewEngineResult.View.RenderAsync(ctx);

        return sw.GetStringBuilder().ToString();
    }
}