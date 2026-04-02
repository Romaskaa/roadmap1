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
    private readonly List<Rule> _rules;

    public RuleCollection()
    {
        _rules = InitializeRules();
    }

    private List<Rule> InitializeRules()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.json");
        var loader = new JsonRuleLoader();
        var builder = new RuleBuilder();
        var jsonRules = loader.Load(filePath);

        return jsonRules
            .Select(builder.Build)
            .OrderBy(rule => rule.Order)
            .ToList();
    }

    public string GetMessage(Profile citizen)
    {
        if (_rules == null)
            return "Подходящих правил не найдено.";

        var applicableRules = new List<Rule>();

        foreach (var rule in _rules)
        {
            if (rule == null)
                continue;

            if (rule.IsApplicable(citizen))
                applicableRules.Add(rule);
        }

        applicableRules.Sort((left, right) => left.Order.CompareTo(right.Order));

        if (!applicableRules.Any())
            return "Подходящих правил не найдено.";

        StringBuilder sb = new StringBuilder();

        foreach (var rule in applicableRules)
            sb.AppendLine(rule.Apply(citizen));

        return sb.ToString();
    }
}
