using System;

namespace LightMemoryDatabase.Api
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CascadeDeleteAttribute : Attribute
    {
    }
}
