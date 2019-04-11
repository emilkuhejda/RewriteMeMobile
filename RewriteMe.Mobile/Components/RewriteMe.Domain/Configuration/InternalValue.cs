namespace RewriteMe.Domain.Configuration
{
    public class InternalValue<T>
    {
        public InternalValue(string key)
            : this(key, default(T))
        { }

        public InternalValue(string key, T defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public string Key { get; }

        public T DefaultValue { get; }
    }
}
