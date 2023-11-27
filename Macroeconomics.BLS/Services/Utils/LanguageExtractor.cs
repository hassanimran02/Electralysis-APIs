using ArgaamPlus.DAL.Entities;
using Common.BLS.Models.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Services.Utils
{
    public static class LanguageExtractor
    {
        public static object? HasProperty(this object obj, string propertyName, LanguagesEnum languagesEnum)
        {
            propertyName += languagesEnum == LanguagesEnum.en ? "En" : "Ar";
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop == null)
                return null;

            return prop.GetValue(obj) ?? "";
        }

        public static string? GetName<TModel>(TModel model, LanguagesEnum languages)
        {
            List<string> expectedNames = new List<string>() { "DisplayName", "Name" };
            foreach (var item in expectedNames)
            {
                var res = model?.HasProperty(item, languages);
                if (res != null)
                    return res.ToString();
            }
            return null;
        }

        public static string? GetValue<TModel>(TModel model, LanguagesEnum languages)
        {
            List<string> expectedNames = new List<string>() { "Value" };
            foreach (var item in expectedNames)
            {
                var res = model?.HasProperty(item, languages);
                if (res != null)
                    return res.ToString();
            }
            return null;
        }
    }
}
