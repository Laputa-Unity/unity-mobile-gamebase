using System.Collections.Generic;

namespace CustomInspector
{
    public class CustomDropdownList<T> : List<CustomDropdownItem<T>>
    {
        public void Add(string text, T value)
        {
            Add(new CustomDropdownItem<T> {Text = text, Value = value,});
        }
    }

    public interface ICustomDropdownItem
    {
        string Text { get; }
        object Value { get; }
    }

    public struct CustomDropdownItem : ICustomDropdownItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
    }

    public struct CustomDropdownItem<T> : ICustomDropdownItem
    {
        public string Text;
        public T Value;

        string ICustomDropdownItem.Text => Text;
        object ICustomDropdownItem.Value => Value;
    }
}