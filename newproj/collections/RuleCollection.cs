using project.Controllers;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using System.IO;
using newproj.Builders;
using newproj.Infrastructure;
using newproj.RuleEngine;

public class RuleCollection
{
    public string GetMessage(Profile citizen)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.json");
        var loader = new JsonRuleLoader();
        var builder = new RuleBuilder();
        var jsonRules = loader.Load(filePath);

        var roadmap = new Roadmap
        {
            Version = "1.0",
            Rules = jsonRules
                .Select(builder.Build)
                .OrderBy(rule => rule.Order)
                .ToList()
        };

        if (roadmap?.Rules == null)
            return "Подходящих правил не найдено.";

        var applicableRules = new List<Rule>();

        foreach (var rule in roadmap.Rules)
        {
            if (rule == null)
                continue;

            if (rule.IsApplicable(citizen))
                applicableRules.Add(rule);
        }

        applicableRules.Sort((l, r) => l.Order.CompareTo(r.Order));

        if (!applicableRules.Any())
            return "Подходящих правил не найдено.";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Версия дорожной карты: {roadmap.Version}");
        sb.AppendLine();

        foreach (var rule in applicableRules)
            sb.AppendLine(rule.Apply(citizen));

        return sb.ToString();
    }
}