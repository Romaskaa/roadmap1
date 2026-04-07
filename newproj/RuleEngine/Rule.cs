using project.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

public class Rule
{
    public string Name { get; set; }
    public int Order { get; set; }
    public Guide Guide { get; set; }
    public List<TargetDocument> TargetDocuments { get; set; }
    public newproj.RuleEngine.RuleDeadlineTrigger Trigger { get; set; }
    public newproj.RuleEngine.IRuleCondition Condition { get; set; }

    public Rule()
    {
        TargetDocuments = new List<TargetDocument>();
    }

    public bool IsApplicable(Profile citizen)
    {
        return Condition == null || Condition.IsSatisfied(citizen);
    }

    public string Apply(Profile citizen)
    {
        var builder = new StringBuilder();
        var deadline = Trigger != null ? Trigger.ResolveDeadline(citizen) : (DateTime?)null;

        builder.AppendLine(Name);
        builder.AppendLine("Что нужно получить:");

        foreach (var document in TargetDocuments)
        { 
            builder.AppendLine($"- {document.Name}");
            var organizations = document.Organizations != null
                ? document.Organizations.Select(o => o.GetInfo()).ToList()
                : new List<string>();

            if (organizations.Any())
                builder.AppendLine($"  Куда обратиться: {string.Join("; ", organizations)}");
        }

        if (Guide != null && !string.IsNullOrWhiteSpace(Guide.Description))
            builder.AppendLine($"Что нужно сделать: {Guide.Description}");

        if (Trigger != null)
        {
            builder.AppendLine($"Срок: {Trigger.Description}");

            if (deadline.HasValue)
                builder.AppendLine($"Крайний срок: {deadline:dd.MM.yyyy}");
        }

        if (deadline.HasValue && DateTime.Now.Date > deadline.Value.Date)
            builder.AppendLine("Статус: срок пропущен.");

        return builder.ToString();
    }
}
