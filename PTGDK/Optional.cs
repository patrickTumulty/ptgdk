namespace PTGDK
{
    /// <summary>
    /// This class is modeled after the Optional type in Java. Should be used when an object might not be present or is
    /// null.
    /// </summary>
    /// <typeparam name="T">The type that might be optional</typeparam>
    public class Optional<T>
    {
        private T value;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Optional value</param>
        private Optional(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Is the optional value present
        /// </summary>
        /// <returns>true if the value is present and not null</returns>
        public bool IsPresent()
        {
            return value != null;
        }

        /// <summary>
        /// Is the optional empty
        /// </summary>
        /// <returns>true if the value is not present or null</returns>
        public bool IsEmpty()
        {
            return !IsPresent();
        }

        /// <summary>
        /// Get the optional value
        /// </summary>
        /// <returns>the value</returns>
        public T Get()
        {
            return value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Optional<T> optional)
            {
                return this.Equals(optional);
            }
            return IsPresent() && value.Equals(obj);
        }

        /// <summary>
        /// Determines is the specified optional object is equal to the current optional object
        /// </summary>
        /// <param name="optional">optional object</param>
        /// <returns>true if both objects are equal</returns>
        public bool Equals(Optional<T> optional)
        {
            if (IsPresent() && optional.IsPresent())
            {
                return value.Equals(optional.Get());
            }
            return false;
        }

        /// <summary>
        /// Create an optional of the given object
        /// </summary>
        /// <param name="obj">the optional object</param>
        /// <returns>An instance of optional/returns>
        public static Optional<object> Of(object obj)
        {
            return new Optional<object>(obj);
        }
    }
}