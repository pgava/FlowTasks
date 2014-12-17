using System;

namespace Flow.Library
{
    /// <summary>
    /// Sql Store Parameter
    /// </summary>
    public class SqlStoreParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public Type ParameterType { get; set; }        
    }
}
