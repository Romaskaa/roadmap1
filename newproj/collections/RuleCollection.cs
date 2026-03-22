using project.Controllers;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class RuleCollection
{
    private readonly List<Rule> _rules;

    public RuleCollection()
    {
        _rules = new List<Rule>();

        InitializeRules();
    }

    private void InitializeRules()
    {
        var allowedCountries = new List<string>
        {
            "Азербайджан",
            "Таджикистан",
            "Узбекистан",
            "Молдова",
            "Украина",
            "Киргизия",
            "Казахстан",
            "Армения"
        };

        var medicalOrg = new MedicalOrganization
        {
            Name = "Областная больница №2",
            Address = "ул. Мельникайте, 75"
        };

        var insuranceOrg = new MedicalOrganization
        {
            Name = "Астро-Волга",
            Address = "Барабинская улица, 28 ст2"
        };

        _rules.Add(new Rule
        {
            Title = "Результат медицинского освидетельствования",
            ActionText = "Вам необходимо обратиться в медицинские организации для прохождения освидетельствования на отсутствие наркомании, инфекционных заболеваний и ВИЧ.",
            DurationDays = 30,
            Organization = medicalOrg,
            Condition = c =>
                c.Purpose?.Name == "Трудовая деятельность"
        });

        _rules.Add(new Rule
        {
            Title = "Результат медицинского освидетельствования",
            ActionText = "Вам необходимо обратиться в медицинские организации для прохождения освидетельствования.",
            DurationDays = 90,
            Organization = medicalOrg,
            Condition = c =>
                c.Purpose?.Name == "Иная"
        });

        _rules.Add(new Rule 
        { 
            Title = "Полис обязательного или дополнительного медицинского страхования", 
            ActionText = "Необходимо обратиться в страховую компанию", 
            DurationDays = 30,
            Organization = insuranceOrg,
            Condition = c => 
                c.Purpose?.Name == "Трудовая деятельность" && 
                c.Citizenship != null && 
                allowedCountries.Contains(c.Citizenship.Name) 
        });
    }

    public string GetMessage(ForeignCitizen citizen)
    {
        var applicableRules = _rules
            .Where(r => r.Condition(citizen))
            .ToList();

        if (!applicableRules.Any())
            return "Подходящих правил не найдено.";

        StringBuilder sb = new StringBuilder();

        foreach (var rule in applicableRules)
            sb.AppendLine(rule.Apply(citizen));

        return sb.ToString();
    }
}