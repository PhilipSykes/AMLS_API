namespace Common.Notification.Email;

public class EmailTemplateProvider
{
    private readonly Dictionary<string, string> _templates = new();
    private readonly string _templateDirectory;

    public EmailTemplateProvider(string templateDirectory = "Content")
    {
        _templateDirectory = templateDirectory;
        LoadTemplates();
    }

    private void LoadTemplates()
    {
        var templateFiles = new Dictionary<string, string>
        {
            {"login", "login-email.html"},
            {"borrow", "borrow-email.html"},
            {"reservation", "reservation-created-email.html"}
        };

        foreach (var template in templateFiles)
        {
            var path = Path.Combine(_templateDirectory, template.Value);
            if (File.Exists(path))
            {
                _templates[template.Key] = File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException($"Email template not found: {path}");
            }
        }
    }

    public string GetTemplate(string templateName)
    {
        if (!_templates.ContainsKey(templateName))
        {
            throw new KeyNotFoundException($"Template not found: {templateName}");
        }
        return _templates[templateName];
    }
}