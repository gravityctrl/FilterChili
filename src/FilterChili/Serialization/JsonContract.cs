using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GravityCTRL.FilterChili.Serialization
{
    public class JsonContract : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            contract.ItemRequired = Required.Always;
            return contract;
        }
    }
}
