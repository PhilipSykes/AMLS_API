namespace Common;

public class DTOs
{
    [Serializable]
    public class Filter
    {
        public string key { get; set; }
        public object value { get; set; }
        public char operation { get; set; }

        public Filter(string key, object value, char operation)
        {
            this.key = key;
            this.value = value;
            this.operation = operation;
        }
    }
}