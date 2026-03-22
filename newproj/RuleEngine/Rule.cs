using project.Controllers;
using System;

public class Rule
{
    public string Title { get; set; }
    public string ActionText { get; set; }
    public int DurationDays { get; set; }
    public MedicalOrganization Organization { get; set; }
    public Func<ForeignCitizen, bool> Condition { get; set; }

    public string Apply(ForeignCitizen citizen)
    {
        DateTime baseDate = citizen.EntryDate;

        if (citizen.ApplicationDate.HasValue)
            baseDate = citizen.ApplicationDate.Value;

        DateTime deadline = baseDate.AddDays(DurationDays);

        if (DateTime.Now.Date > deadline.Date)
            return $"{Title}\nСрок пропущен.\n";

        string orgInfo = Organization != null
            ? $"Куда обратиться: {Organization.GetInfo()}\n"
            : "";

        return
            $"{Title}\n" +
            $"Что нужно сделать: {ActionText}\n" +
            orgInfo +
            $"Срок до: {deadline:dd.MM.yyyy}\n";
    }
}