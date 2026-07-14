namespace MacroViva.Api.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequiresTesterIdentityAttribute : Attribute;
