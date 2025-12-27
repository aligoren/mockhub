using Scriban;
using Scriban.Runtime;
using MockHub.MockEngine.Variables;

namespace MockHub.MockEngine.Templates;

public class TemplateEngine
{
    private readonly DynamicVariableProvider _variableProvider;

    public TemplateEngine()
    {
        _variableProvider = new DynamicVariableProvider();
    }

    public string Render(string template, MockRequestContext context)
    {
        if (string.IsNullOrEmpty(template))
        {
            return template;
        }

        // First pass: Replace {{faker.xxx}} and other dynamic syntax
        template = ReplaceFakerVariables(template, context);

        // Second pass: Replace {{$variable}} syntax  
        template = ReplaceDynamicVariables(template);

        // Third pass: Use Scriban for complex templates
        return RenderWithScriban(template, context);
    }

    private string ReplaceFakerVariables(string template, MockRequestContext context)
    {
        // Match {{faker.category.method}} style
        var fakerPattern = @"\{\{\s*faker\.([a-zA-Z\.]+(?:\([^)]*\))?)\s*\}\}";
        template = System.Text.RegularExpressions.Regex.Replace(template, fakerPattern, match =>
        {
            var expression = match.Groups[1].Value;
            return _variableProvider.GetFakerValue(expression) ?? match.Value;
        });

        // Match {{now}}, {{nowUnix}}, {{uuid}} etc.
        var simpleVarPattern = @"\{\{\s*(now|nowUnix|nowMs|uuid|guid|today)\s*\}\}";
        template = System.Text.RegularExpressions.Regex.Replace(template, simpleVarPattern, match =>
        {
            var varName = match.Groups[1].Value;
            return varName.ToLower() switch
            {
                "now" => DateTime.UtcNow.ToString("O"),
                "nowunix" => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                "nowms" => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                "uuid" or "guid" => Guid.NewGuid().ToString(),
                "today" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
                _ => match.Value
            };
        });

        // Match {{request.params.xxx}}, {{request.query.xxx}}, {{request.body.xxx}}, {{request.headers.xxx}}
        var requestPattern = @"\{\{\s*request\.(params|query|headers|body)\.([a-zA-Z0-9_]+)\s*\}\}";
        template = System.Text.RegularExpressions.Regex.Replace(template, requestPattern, match =>
        {
            var location = match.Groups[1].Value.ToLower();
            var key = match.Groups[2].Value;

            return location switch
            {
                "params" => context.RouteParams.TryGetValue(key, out var pv) ? pv : match.Value,
                "query" => context.QueryParams.TryGetValue(key, out var qv) ? qv : match.Value,
                "headers" => context.Headers.TryGetValue(key, out var hv) ? hv : match.Value,
                "body" => GetBodyValue(context.Body, key) ?? match.Value,
                _ => match.Value
            };
        });

        // Match {{request.id}}, {{request.method}}, {{request.path}}
        var requestSimplePattern = @"\{\{\s*request\.(id|method|path)\s*\}\}";
        template = System.Text.RegularExpressions.Regex.Replace(template, requestSimplePattern, match =>
        {
            var prop = match.Groups[1].Value.ToLower();
            return prop switch
            {
                "id" => Guid.NewGuid().ToString("N")[..12],
                "method" => context.Method,
                "path" => context.Path,
                _ => match.Value
            };
        });

        return template;
    }

    private static string? GetBodyValue(object? body, string key)
    {
        if (body == null) return null;

        if (body is System.Text.Json.JsonElement jsonElement)
        {
            if (jsonElement.TryGetProperty(key, out var prop))
            {
                return prop.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => prop.GetString(),
                    System.Text.Json.JsonValueKind.Number => prop.GetRawText(),
                    System.Text.Json.JsonValueKind.True => "true",
                    System.Text.Json.JsonValueKind.False => "false",
                    _ => prop.GetRawText()
                };
            }
        }
        else if (body is Dictionary<string, object> dict)
        {
            return dict.TryGetValue(key, out var val) ? val?.ToString() : null;
        }

        return null;
    }

    private string ReplaceDynamicVariables(string template)
    {
        // Match {{$variableName}} or {{$variableName(args)}}
        var pattern = @"\{\{\s*\$(\w+(?:\([^)]*\))?)\s*\}\}";
        
        return System.Text.RegularExpressions.Regex.Replace(template, pattern, match =>
        {
            var expression = match.Groups[1].Value;
            return _variableProvider.GenerateValue(expression) ?? match.Value;
        });
    }

    private string RenderWithScriban(string template, MockRequestContext context)
    {
        try
        {
            // Check if template has Scriban syntax
            if (!template.Contains("{{") || !template.Contains("}}"))
            {
                return template;
            }

            var scribanTemplate = Template.Parse(template);
            
            if (scribanTemplate.HasErrors)
            {
                return template;
            }

            var scriptObject = new ScriptObject();
            
            // Add request context
            scriptObject.Add("request", new
            {
                path = context.Path,
                method = context.Method,
                query = context.QueryParams,
                headers = context.Headers,
                body = context.Body,
                route = context.RouteParams,
                id = Guid.NewGuid().ToString("N")[..12]
            });

            // Add helper functions
            scriptObject.Import("now", new Func<string>(() => DateTime.UtcNow.ToString("O")));
            scriptObject.Import("guid", new Func<string>(() => Guid.NewGuid().ToString()));
            scriptObject.Import("random_int", new Func<int, int, int>((min, max) => Random.Shared.Next(min, max)));
            scriptObject.Import("random_string", new Func<int, string>(length => 
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
            }));

            var templateContext = new TemplateContext();
            templateContext.PushGlobal(scriptObject);

            return scribanTemplate.Render(templateContext);
        }
        catch
        {
            return template;
        }
    }
}

public class MockRequestContext
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public Dictionary<string, string> QueryParams { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    public Dictionary<string, string> RouteParams { get; set; } = new();
    public object? Body { get; set; }
    public string? RawBody { get; set; }
}
