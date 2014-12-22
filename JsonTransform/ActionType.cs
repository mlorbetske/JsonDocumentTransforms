using System.Runtime.Serialization;

namespace JsonTransform
{
    public enum ActionType
    {
        [EnumMember(Value = "insert")]
        Insert,
        [EnumMember(Value = "replace")]
        Replace
    }
}