using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Ultilities
{
    public static class UltilitiesService
    {
        public static void AddOrUpdateError(Dictionary<string, List<string>> dict, string key, string value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(value);
            }
            else
            {
                List<string> valueList = new List<string> { value };
                dict.Add(key, valueList);
            }
        }
    }
}
