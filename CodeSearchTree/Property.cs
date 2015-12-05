using System.Text.RegularExpressions;

namespace CodeSearchTree
{
    public class Property
    {
        internal Property(string name, object value)
        {
            PropertyName = name;
            PropertyValue = value;
        }

        public string PropertyName { get; }
        public object PropertyValue { get; }

        public string PropertyValueString
        {
            get
            {
                var propValueString = PropertyValue?.ToString() ?? "";
                propValueString = Regex.Replace(propValueString, @"\s+", " ").Trim();
                return propValueString.Length > 200
                    ? propValueString.Substring(0, 200).Trim() + "..."
                    : propValueString;
            }
        }

        public override string ToString() => $"{PropertyName}: {PropertyValueString}";
    }
}