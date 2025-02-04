using System.Text;
using System.Text.RegularExpressions;
using Application.Contracts;

namespace Application.Services;

public class UtilityService : BaseService, IUtilityService
{
    public string NormalizeText(string value)
    {
        var normalizedValue = value
            .Trim()
            .ToUpperInvariant()
            .Normalize(NormalizationForm.FormC);

        return Regex.Replace(
            normalizedValue,
            @"\s+", "_"
        );
    }
}
