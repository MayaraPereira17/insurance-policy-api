using System.Text.RegularExpressions;

namespace InsurancePolicy.Application.Validators;

public static partial class DocumentoSeguradoValidator
{
    private static readonly int[] PesosDv = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private const string CnpjZerado = "00000000000000";

    public static string ValidarENormalizar(string documento)
    {
        var normalizado = Normalizar(documento);

        return normalizado.Length switch
        {
            11 when normalizado.All(char.IsDigit) => normalizado,
            14 when CnpjValido(normalizado) => normalizado,
            11 => throw new ArgumentException("CPF deve conter apenas números."),
            14 => throw new ArgumentException("CNPJ inválido. Verifique o número e os dígitos verificadores."),
            _ => throw new ArgumentException("CPF deve ter 11 dígitos ou CNPJ deve ter 14 caracteres.")
        };
    }

    public static string Normalizar(string documento)
    {
        if (CaracteresInvalidos().IsMatch(documento))
            throw new ArgumentException("CPF/CNPJ contém caracteres inválidos.");

        return MascaraDocumento().Replace(documento, string.Empty).ToUpperInvariant();
    }

    private static bool CnpjValido(string cnpj)
    {
        if (!FormatoCnpj().IsMatch(cnpj) || cnpj == CnpjZerado)
            return false;

        var baseCnpj = cnpj[..12];
        var dvInformado = cnpj[12..];
        var dvCalculado = CalcularDigitosVerificadoresCnpj(baseCnpj);

        return dvInformado == dvCalculado;
    }

    private static string CalcularDigitosVerificadoresCnpj(string baseCnpj)
    {
        var somatorioDv1 = 0;
        var somatorioDv2 = 0;

        for (var i = 0; i < 12; i++)
        {
            var valor = ValorParaCalculoDv(baseCnpj[i]);
            somatorioDv1 += valor * PesosDv[i + 1];
            somatorioDv2 += valor * PesosDv[i];
        }

        var dv1 = CalcularDigitoVerificador(somatorioDv1);
        somatorioDv2 += dv1 * PesosDv[12];
        var dv2 = CalcularDigitoVerificador(somatorioDv2);

        return $"{dv1}{dv2}";
    }

    private static int ValorParaCalculoDv(char caractere) => caractere - '0';

    private static int CalcularDigitoVerificador(int somatorio)
    {
        var resto = somatorio % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    [GeneratedRegex(@"[^A-Z0-9./-]", RegexOptions.IgnoreCase)]
    private static partial Regex CaracteresInvalidos();

    [GeneratedRegex(@"[./-]")]
    private static partial Regex MascaraDocumento();

    [GeneratedRegex(@"^[A-Z0-9]{12}\d{2}$")]
    private static partial Regex FormatoCnpj();
}
