using System;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomValueDrawerAttribute : Attribute
    {
        public RegisterCustomValueDrawerAttribute(Type drawerType, int order)
        {
            DrawerType = drawerType;
            Order = order;
        }

        public Type DrawerType { get; }
        public int Order { get; }
        public bool ApplyOnArrayElement { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomAttributeDrawerAttribute : Attribute
    {
        public RegisterCustomAttributeDrawerAttribute(Type drawerType, int order)
        {
            DrawerType = drawerType;
            Order = order;
        }

        public Type DrawerType { get; }
        public int Order { get; }
        public bool ApplyOnArrayElement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomGroupDrawerAttribute : Attribute
    {
        public RegisterCustomGroupDrawerAttribute(Type drawerType)
        {
            DrawerType = drawerType;
        }

        public Type DrawerType { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomPropertyHideProcessor : Attribute
    {
        public RegisterCustomPropertyHideProcessor(Type processorType)
        {
            ProcessorType = processorType;
        }

        public Type ProcessorType { get; }
        public bool ApplyOnArrayElement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomPropertyDisableProcessor : Attribute
    {
        public RegisterCustomPropertyDisableProcessor(Type processorType)
        {
            ProcessorType = processorType;
        }

        public Type ProcessorType { get; }
        public bool ApplyOnArrayElement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomValueValidatorAttribute : Attribute
    {
        public RegisterCustomValueValidatorAttribute(Type validatorType)
        {
            ValidatorType = validatorType;
        }

        public Type ValidatorType { get; }
        public bool ApplyOnArrayElement { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomAttributeValidatorAttribute : Attribute
    {
        public RegisterCustomAttributeValidatorAttribute(Type validatorType)
        {
            ValidatorType = validatorType;
        }

        public Type ValidatorType { get; }
        public bool ApplyOnArrayElement { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomTypeProcessorAttribute : Attribute
    {
        public RegisterCustomTypeProcessorAttribute(Type processorType, int order)
        {
            ProcessorType = processorType;
            Order = order;
        }

        public Type ProcessorType { get; }
        public int Order { get; }
    }
}