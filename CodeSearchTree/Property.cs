using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
   public class Property
   {
      public string PropertyName { get; private set; }
      public object PropertyValue { get; private set; }

      internal Property(string name, object value)
      {
         this.PropertyName = name;
         this.PropertyValue = value;
      }

      public string PropertyValueString
      {
         get
         {
            var prop_value_string = this.PropertyValue == null ? "" : this.PropertyValue.ToString();
            prop_value_string = System.Text.RegularExpressions.Regex.Replace(prop_value_string, @"\s+", " ").Trim();
            return prop_value_string.Length > 200 ? (prop_value_string.Substring(0, 200).Trim() + "...") : prop_value_string;
         }
      }

      public override string ToString() => $"{this.PropertyName}: {this.PropertyValueString}";

   }
}
