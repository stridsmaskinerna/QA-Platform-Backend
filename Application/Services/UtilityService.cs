using System.Text;
using System.Text.RegularExpressions;

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
