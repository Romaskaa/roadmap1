using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.Controllers;

namespace newproj.RuleEngine
{
    public class CompositeRuleCondition : IRuleCondition
    {
        private readonly List<IRuleCondition> _conditions;

        public CompositeRuleCondition()
        {
            _conditions = new List<IRuleCondition>();
        }

        public CompositeRuleCondition Add(IRuleCondition condition)
        {
            if (condition != null)
                _conditions.Add(condition);

            return this;
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            return _conditions.All(condition => condition.IsSatisfied(citizen));
        }
    }

    public class ForeignCitizenCondition : IRuleCondition
    {
        private readonly bool _expectedValue;

        public ForeignCitizenCondition(bool expectedValue)
        {
            _expectedValue = expectedValue;
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            return citizen != null && citizen.IsForeignCitizen == _expectedValue;
        }
    }

    public class PurposeCondition : IRuleCondition
    {
        private readonly HashSet<string> _purposes;

        public PurposeCondition(IEnumerable<string> purposes)
        {
            _purposes = new HashSet<string>(purposes ?? Enumerable.Empty<string>());
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            return citizen != null &&
                   citizen.Purpose != null &&
                   _purposes.Contains(citizen.Purpose.Name);
        }
    }

    public class CitizenshipCondition : IRuleCondition
    {
        private readonly HashSet<string> _citizenships;

        public CitizenshipCondition(IEnumerable<string> citizenships)
        {
            _citizenships = new HashSet<string>(citizenships ?? Enumerable.Empty<string>());
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            return citizen != null &&
                   citizen.Citizenship != null &&
                   _citizenships.Contains(citizen.Citizenship.Name);
        }
    }

    public class StayDurationCondition : IRuleCondition
    {
        private readonly int? _minExclusive;
        private readonly int? _maxInclusive;

        public StayDurationCondition(int? minExclusive, int? maxInclusive)
        {
            _minExclusive = minExclusive;
            _maxInclusive = maxInclusive;
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            if (citizen == null || !citizen.DurationDays.HasValue)
                return false;

            var value = citizen.DurationDays.Value;

            if (_minExclusive.HasValue && value <= _minExclusive.Value)
                return false;

            if (_maxInclusive.HasValue && value > _maxInclusive.Value)
                return false;

            return true;
        }
    }

    public class ApplicationPresenceCondition : IRuleCondition
    {
        private readonly bool _required;

        public ApplicationPresenceCondition(bool required)
        {
            _required = required;
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            var hasApplication = citizen != null && citizen.HasApplication();
            return hasApplication == _required;
        }
    }

    public class ProfilePropertyCondition : IRuleCondition
    {
        private readonly string _propertyName;
        private readonly string _expectedValue;

        public ProfilePropertyCondition(string propertyName, string expectedValue)
        {
            _propertyName = propertyName;
            _expectedValue = expectedValue;
        }

        public bool IsSatisfied(ForeignCitizen citizen)
        {
            if (citizen == null)
                return false;

            var actualValue = citizen.GetPropertyValue(_propertyName);
            return string.Equals(actualValue, _expectedValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
