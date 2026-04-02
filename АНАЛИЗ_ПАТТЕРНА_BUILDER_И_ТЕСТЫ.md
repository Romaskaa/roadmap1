# Анализ соответствия паттерну Builder и рекомендации по тестированию

## ?? АНАЛИЗ ПАТТЕРНА BUILDER

### ? Правильное использование паттерна Builder

**Класс: `RuleBuilder.cs`** полностью соответствует паттерну Builder:

#### 1. **Структура паттерна**
```csharp
public class RuleBuilder
{
    // Build() - основной метод для создания объекта
    public Rule Build(JsonRule jsonRule)
    {
        return new Rule
        {
            // конструирует объект из DTO
            Name = jsonRule.Name,
            Order = jsonRule.Order,
            Guide = BuildGuide(jsonRule.Guide),
            TargetDocuments = BuildTargetDocuments(jsonRule.TargetDocuments),
            Trigger = BuildTrigger(jsonRule.Trigger),
            Condition = BuildCondition(jsonRule.Conditions)
        };
    }
}
```

#### 2. **Выделенные вспомогательные методы**
- `BuildGuide()` - конструирует Guide с Organizations
- `BuildTargetDocuments()` - конструирует список TargetDocument
- `BuildTrigger()` - конструирует RuleDeadlineTrigger
- `BuildCondition()` - конструирует комплексную иерархию условий
- `ParseAnchor()` - вспомогательный метод для парсинга якоря

#### 3. **Fluent Interface (Fluent Builder Pattern)**
```csharp
public class CompositeRuleCondition : IRuleCondition
{
    // Возвращает this для цепочки вызовов
    public CompositeRuleCondition Add(IRuleCondition condition)
    {
        if (condition != null)
            _conditions.Add(condition);
        return this;  // ? Fluent pattern
    }
}
```

Использование:
```csharp
var builder = new CompositeRuleCondition();
builder.Add(condition1)
       .Add(condition2)
       .Add(condition3);
```

---

### ?? ПРОБЛЕМЫ И ВОЗМОЖНЫЕ УЛУЧШЕНИЯ

#### 1. **Недостаточная валидация в BuildCondition()**
```csharp
private IRuleCondition BuildCondition(JsonRuleCondition conditions)
{
    var builder = new CompositeRuleCondition();
    
    if (conditions == null)
        return builder;  // ? Правильно обрабатывает null
    
    // Но что если conditions.ProfileProperties содержит null элементы?
    if (conditions.ProfileProperties != null)
    {
        foreach (var propertyCondition in conditions.ProfileProperties)
        {
            if (propertyCondition.Values != null && propertyCondition.Values.Any())
            {
                // ?? propertyCondition может быть null
                builder.Add(new ProfilePropertyInSetCondition(...));
            }
        }
    }
}
```

**Рекомендация:**
```csharp
if (conditions.ProfileProperties != null)
{
    foreach (var propertyCondition in conditions.ProfileProperties?.Where(p => p != null))
    {
        // Безопасно работаем с propertyCondition
    }
}
```

#### 2. **ParseAnchor() - нарушение принципа Open/Closed**
```csharp
private DeadlineAnchor ParseAnchor(string anchor)
{
    if (string.Equals(anchor, "application", StringComparison.OrdinalIgnoreCase))
        return DeadlineAnchor.ApplicationDate;
    
    if (string.Equals(anchor, "entry_or_application", StringComparison.OrdinalIgnoreCase))
        return DeadlineAnchor.EntryOrApplicationDate;
    
    return DeadlineAnchor.EntryDate;  // Default
}
```

**Проблема:** Добавление новых значений требует изменения метода.

**Рекомендация - использовать Dictionary:**
```csharp
private static readonly Dictionary<string, DeadlineAnchor> AnchorMapping = 
    new Dictionary<string, DeadlineAnchor>(StringComparer.OrdinalIgnoreCase)
    {
        { "application", DeadlineAnchor.ApplicationDate },
        { "entry_or_application", DeadlineAnchor.EntryOrApplicationDate },
        { "entry", DeadlineAnchor.EntryDate }
    };

private DeadlineAnchor ParseAnchor(string anchor)
{
    return AnchorMapping.TryGetValue(anchor ?? "entry", out var result)
        ? result
        : DeadlineAnchor.EntryDate;
}
```

#### 3. **Жесткая зависимость от конкретных классов**
```csharp
public Rule Build(JsonRule jsonRule)
{
    return new Rule  // ? Жесткая зависимость
    {
        // ...
        Guide = BuildGuide(jsonRule.Guide),
        Condition = BuildCondition(jsonRule.Conditions)
    };
}
```

**Рекомендация - использовать Factory Pattern:**
```csharp
public interface IRuleFactory
{
    Rule CreateRule(JsonRule jsonRule);
    Guide CreateGuide(JsonGuide jsonGuide);
}
```

#### 4. **Отсутствие логирования ошибок**
```csharp
public Rule Build(JsonRule jsonRule)
{
    if (jsonRule == null)
        throw new ArgumentNullException("jsonRule");
    // ? Нет информации о Context при ошибке
}
```

---

## ?? МОДУЛЬНЫЕ ТЕСТЫ

Ниже представлены примеры модульных тестов для программы. 

> **Примечание:** Из-за конфигурации проекта (Exe вместо Class Library), полная интеграция MSTest требует создания отдельного проекта Unit Tests Project. Ниже приведены полные примеры тестов, готовые к использованию в отдельном проекте.

### 1?? RuleBuilderTests.cs

**Покрытие:**
- Null value handling
- Property mapping
- Complex object construction
- Edge cases

```csharp
[TestClass]
public class RuleBuilderTests
{
    private RuleBuilder _builder;

    [TestInitialize]
    public void Setup()
    {
        _builder = new RuleBuilder();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Build_WithNullJsonRule_ThrowsArgumentNullException()
    {
        _builder.Build(null);
    }

    [TestMethod]
    public void Build_WithValidJsonRule_ReturnsRuleWithCorrectProperties()
    {
        var jsonRule = new JsonRule { Name = "Test Rule", Order = 1 };
        var result = _builder.Build(jsonRule);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Test Rule", result.Name);
        Assert.AreEqual(1, result.Order);
    }

    [TestMethod]
    public void Build_WithGuideAndOrganizations_CreatesCorrectGuide()
    {
        var jsonRule = new JsonRule
        {
            Name = "Test",
            Order = 1,
            Guide = new JsonGuide
            {
                Description = "Test description",
                Organizations = new List<JsonOrganization>
                {
                    new JsonOrganization { Name = "Org1", Address = "Address1" },
                    new JsonOrganization { Name = "Org2", Address = "Address2" }
                }
            }
        };

        var result = _builder.Build(jsonRule);

        Assert.AreEqual(2, result.Guide.Organizations.Count);
        Assert.AreEqual("Org1", result.Guide.Organizations[0].Name);
    }

    [TestMethod]
    public void Build_WithAnchorApplication_ParsesCorrectly()
    {
        var jsonRule = new JsonRule
        {
            Name = "Test",
            Order = 1,
            Trigger = new JsonDeadlineTrigger { Anchor = "application" }
        };

        var result = _builder.Build(jsonRule);

        Assert.AreEqual(DeadlineAnchor.ApplicationDate, result.Trigger.Anchor);
    }

    [TestMethod]
    public void Build_WithUnknownAnchor_DefaultsToEntryDate()
    {
        var jsonRule = new JsonRule
        {
            Name = "Test",
            Order = 1,
            Trigger = new JsonDeadlineTrigger { Anchor = "unknown" }
        };

        var result = _builder.Build(jsonRule);

        Assert.AreEqual(DeadlineAnchor.EntryDate, result.Trigger.Anchor);
    }
}
```

### 2?? RuleDeadlineTriggerTests.cs

**Покрытие:**
- Deadline calculation with different anchors
- Date manipulation
- Null handling

```csharp
[TestClass]
public class RuleDeadlineTriggerTests
{
    private RuleDeadlineTrigger _trigger;

    [TestInitialize]
    public void Setup()
    {
        _trigger = new RuleDeadlineTrigger();
    }

    [TestMethod]
    public void ResolveDeadline_WithEntryDateAnchor_CalculatesCorrectDeadline()
    {
        var entryDate = DateTime.Now.AddDays(-10);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        _trigger.Anchor = DeadlineAnchor.EntryDate;
        _trigger.Days = 30;

        var deadline = _trigger.ResolveDeadline(citizen);

        Assert.IsNotNull(deadline);
        Assert.AreEqual(entryDate.AddDays(30).Date, deadline.Value.Date);
    }

    [TestMethod]
    public void ResolveDeadline_WithApplicationDateAnchor_CalculatesCorrectDeadline()
    {
        var entryDate = DateTime.Now.AddDays(-20);
        var applicationDate = DateTime.Now.AddDays(-10);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o")),
            new ProfileProperty(Profile.ApplicationDatePropertyName, applicationDate.ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        _trigger.Anchor = DeadlineAnchor.ApplicationDate;
        _trigger.Days = 30;

        var deadline = _trigger.ResolveDeadline(citizen);

        Assert.IsNotNull(deadline);
        Assert.AreEqual(applicationDate.AddDays(30).Date, deadline.Value.Date);
    }

    [TestMethod]
    public void ResolveDeadline_WithApplicationDateAnchorButNoApplication_ReturnsNull()
    {
        var entryDate = DateTime.Now.AddDays(-10);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        _trigger.Anchor = DeadlineAnchor.ApplicationDate;
        _trigger.Days = 30;

        var deadline = _trigger.ResolveDeadline(citizen);

        Assert.IsNull(deadline);
    }

    [TestMethod]
    public void ResolveDeadline_WithEntryOrApplicationAnchorAndApplication_UsesApplicationDate()
    {
        var entryDate = DateTime.Now.AddDays(-20);
        var applicationDate = DateTime.Now.AddDays(-5);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o")),
            new ProfileProperty(Profile.ApplicationDatePropertyName, applicationDate.ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        _trigger.Anchor = DeadlineAnchor.EntryOrApplicationDate;
        _trigger.Days = 30;

        var deadline = _trigger.ResolveDeadline(citizen);

        Assert.IsNotNull(deadline);
        Assert.AreEqual(applicationDate.AddDays(30).Date, deadline.Value.Date);
    }
}
```

### 3?? RuleConditionTests.cs

**Покрытие:**
- Condition evaluation logic
- Composite pattern
- Edge cases

```csharp
[TestClass]
public class RuleConditionTests
{
    [TestMethod]
    public void ForeignCitizenCondition_WithForeignCitizen_ReturnsTrue()
    {
        var condition = new ForeignCitizenCondition(true);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
        };
        var citizen = new Profile(null, "Test", properties);

        var result = condition.IsSatisfied(citizen);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ApplicationPresenceCondition_WithRequired_VerifiesPresence()
    {
        var condition = new ApplicationPresenceCondition(required: true);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o")),
            new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.AddDays(-5).ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        var result = condition.IsSatisfied(citizen);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CompositeRuleCondition_WithMultipleConditions_AllMustBeSatisfied()
    {
        var composite = new CompositeRuleCondition();
        composite.Add(new ForeignCitizenCondition(true));
        composite.Add(new ApplicationPresenceCondition(true));

        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "USA"),
            new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o")),
            new ProfileProperty(Profile.ApplicationDatePropertyName, DateTime.Now.AddDays(-5).ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        var result = composite.IsSatisfied(citizen);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CompositeRuleCondition_WithOneFailingCondition_ReturnsFalse()
    {
        var composite = new CompositeRuleCondition();
        composite.Add(new ForeignCitizenCondition(true));
        composite.Add(new ApplicationPresenceCondition(true));

        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "USA"),
            new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
            // Missing application date
        };
        var citizen = new Profile(null, "Test", properties);

        var result = composite.IsSatisfied(citizen);

        Assert.IsFalse(result);
    }
}
```

### 4?? ProfileTests.cs

**Покрытие:**
- Property management
- Date handling
- State validation

```csharp
[TestClass]
public class ProfileTests
{
    [TestMethod]
    public void IsForeignCitizen_WithNonRussianCitizenship_ReturnsTrue()
    {
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
        };
        var profile = new Profile(null, "Test", properties);

        Assert.IsTrue(profile.IsForeignCitizen);
    }

    [TestMethod]
    public void IsForeignCitizen_WithRussianCitizenship_ReturnsFalse()
    {
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "РФ")
        };
        var profile = new Profile(null, "Test", properties);

        Assert.IsFalse(profile.IsForeignCitizen);
    }

    [TestMethod]
    public void GetEntryDate_WithValidDate_ReturnsCorrectDate()
    {
        var entryDate = DateTime.Now.AddDays(-10);
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, entryDate.ToString("o"))
        };
        var profile = new Profile(null, "Test", properties);

        var result = profile.GetEntryDate();

        Assert.AreEqual(entryDate.Date, result.Date);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetEntryDate_WithoutEntryDate_ThrowsInvalidOperationException()
    {
        var profile = new Profile(null, "Test", new List<ProfileProperty>());

        profile.GetEntryDate();
    }

    [TestMethod]
    public void SetProperty_NewProperty_AddsToList()
    {
        var profile = new Profile(null, "Test", new List<ProfileProperty>());

        profile.SetProperty("TestProp", "TestValue");

        Assert.AreEqual("TestValue", profile.GetPropertyValue("TestProp"));
    }

    [TestMethod]
    public void SetProperty_ExistingProperty_UpdatesValue()
    {
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty("TestProp", "OldValue")
        };
        var profile = new Profile(null, "Test", properties);

        profile.SetProperty("TestProp", "NewValue");

        Assert.AreEqual("NewValue", profile.GetPropertyValue("TestProp"));
    }
}
```

### 5?? RuleTests.cs

**Покрытие:**
- Rule application logic
- Message generation
- Condition evaluation

```csharp
[TestClass]
public class RuleTests
{
    [TestMethod]
    public void IsApplicable_WithNullCondition_ReturnsTrue()
    {
        var rule = new Rule { Condition = null };
        var citizen = new Profile(null, "Test", new List<ProfileProperty>());

        var result = rule.IsApplicable(citizen);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsApplicable_WithSatisfiedCondition_ReturnsTrue()
    {
        var condition = new ForeignCitizenCondition(true);
        var rule = new Rule { Condition = condition };
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.CitizenshipPropertyName, "USA")
        };
        var citizen = new Profile(null, "Test", properties);

        var result = rule.IsApplicable(citizen);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Apply_WithBasicRule_GeneratesCorrectMessage()
    {
        var rule = new Rule
        {
            Name = "Get Passport",
            Order = 1,
            TargetDocuments = new List<TargetDocument>
            {
                new TargetDocument("Passport")
            }
        };
        var citizen = new Profile(null, "Test", new List<ProfileProperty>());

        var result = rule.Apply(citizen);

        Assert.IsTrue(result.Contains("Get Passport"));
        Assert.IsTrue(result.Contains("Passport"));
    }

    [TestMethod]
    public void Apply_WithDeadline_IncludesDeadlineInfo()
    {
        var trigger = new RuleDeadlineTrigger
        {
            Anchor = DeadlineAnchor.EntryDate,
            Days = 30,
            Description = "30 days from entry"
        };
        var rule = new Rule
        {
            Name = "Test Rule",
            Order = 1,
            Trigger = trigger
        };
        var properties = new List<ProfileProperty>
        {
            new ProfileProperty(Profile.EntryDatePropertyName, DateTime.Now.AddDays(-10).ToString("o"))
        };
        var citizen = new Profile(null, "Test", properties);

        var result = rule.Apply(citizen);

        Assert.IsTrue(result.Contains("30 days from entry"));
    }
}
```

### 6?? JsonRuleLoaderTests.cs

**Покрытие:**
- File I/O
- JSON parsing
- Error handling

```csharp
[TestClass]
public class JsonRuleLoaderTests
{
    private JsonRuleLoader _loader;
    private string _testFilePath;

    [TestInitialize]
    public void Setup()
    {
        _loader = new JsonRuleLoader();
        _testFilePath = Path.Combine(Path.GetTempPath(), "test_rules.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Load_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "non_existent_rules.json");
        _loader.Load(nonExistentPath);
    }

    [TestMethod]
    public void Load_WithValidJsonFile_ReturnsRulesList()
    {
        var json = @"[{""Name"": ""Test Rule"", ""Order"": 1}]";
        File.WriteAllText(_testFilePath, json, Encoding.UTF8);

        var result = _loader.Load(_testFilePath);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test Rule", result[0].Name);
    }

    [TestMethod]
    public void Load_WithUtf8Content_HandlesCorrectly()
    {
        var json = @"[{""Name"": ""Получить визу"", ""Order"": 1}]";
        File.WriteAllText(_testFilePath, json, Encoding.UTF8);

        var result = _loader.Load(_testFilePath);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Получить визу", result[0].Name);
    }
}
```

---

## ?? КАК ИСПОЛЬЗОВАТЬ ТЕСТЫ

### Вариант 1: Создание отдельного проекта Unit Tests (рекомендуется)

```bash
# В Visual Studio:
# 1. Правый клик на Solution
# 2. Add ? New Project
# 3. Выбрать "Unit Test Project (.NET Framework)"
# 4. Назвать его "newproj.Tests"
# 5. Добавить reference на основной проект newproj
# 6. Скопировать тесты в проект
# 7. Run ? Run Tests
```

### Вариант 2: Использование NUnit (альтернатива MSTest)

Если MSTest недоступен, используйте **NUnit**:

```csharp
using NUnit.Framework;

[TestFixture]
public class RuleBuilderTests
{
    [Test]
    public void Build_WithValidRule_ReturnsCorrectly()
    {
        // Arrange
        var builder = new RuleBuilder();
        var jsonRule = new JsonRule { Name = "Test", Order = 1 };
        
        // Act
        var result = builder.Build(jsonRule);
        
        // Assert
        Assert.That(result.Name, Is.EqualTo("Test"));
    }
}
```

---

## ?? ИТОГОВАЯ ОЦЕНКА

| Аспект | Оценка | Комментарий |
|--------|--------|-----------|
| Соответствие паттерну Builder | ? 9/10 | Отличная реализация, minor improvements возможны |
| Fluent Interface | ? 10/10 | Идеально реализовано в CompositeRuleCondition |
| Error Handling | ?? 7/10 | Базовая валидация, можно улучшить |
| Testability | ? 8/10 | Хорошая структура для тестирования |
| Extensibility | ?? 6/10 | ParseAnchor требует изменений при расширении |
| Code Reusability | ? 8/10 | Хорошее разделение ответственности |

---

## ?? РЕКОМЕНДАЦИИ

1. **Применить улучшения к ParseAnchor()** - использовать Dictionary вместо цепи if
2. **Добавить обработку null в collections** - проверить null элементы массивов
3. **Создать отдельный проект Unit Tests** для полного тестового покрытия
4. **Добавить логирование** - используйте log4net или встроенный ILogger
5. **Документировать исключения** - добавьте XML comments с описанием throw cases
