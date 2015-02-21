// Guids.cs
// MUST match guids.h

using System;

namespace JsonTransform.VisualStudio
{
    static class GuidList
    {
        public const string guidJsonTransform_VisualStudioPkgString = "f4319086-d68e-4e8a-adc1-4535fefa97a9";
        public const string guidJsonTransform_VisualStudioCmdSetString = "ce03a154-c216-4af7-97b1-c24e044e2420";

        public static readonly Guid guidJsonTransform_VisualStudioCmdSet = new Guid(guidJsonTransform_VisualStudioCmdSetString);
    };
}