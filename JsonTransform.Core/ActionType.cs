using System.Runtime.Serialization;

namespace JsonTransform.Core
{
    public enum ActionType
    {
        [EnumMember(Value = "insert")]
        Insert,
        [EnumMember(Value = "replace")]
        Replace
    }
}