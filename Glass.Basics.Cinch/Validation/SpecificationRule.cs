using Cinch;

namespace Glass.Basics.Cinch.Validation
{
    public class SpecificationRule<T> : Rule where T : class {
        private readonly ISpecification<T> specification;

        public SpecificationRule(string propertyName, string brokenDescription, ISpecification<T> specification)
            : base(propertyName, brokenDescription) {
            this.specification = specification;
        }

        public override bool ValidateRule(object domainObject) {
            var pi = domainObject.GetType().GetProperty(PropertyName);
            var value = pi.GetValue(domainObject, null) as T;

            if (value != null) {
                return !specification.IsSatisfiedBy(value);
            }
            return false;
        }
    }
}
