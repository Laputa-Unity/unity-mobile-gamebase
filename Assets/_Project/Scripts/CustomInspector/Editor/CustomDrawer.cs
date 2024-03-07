namespace CustomInspector
{
    public abstract class CustomDrawer : CustomPropertyExtension
    {
        internal int Order { get; set; }

        public abstract CustomElement CreateElementInternal(CustomProperty property, CustomElement next);
    }
}