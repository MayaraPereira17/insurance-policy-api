using InsurancePolicy.Application.Validators;

namespace InsurancePolicy.Tests.Validators;

public class DocumentoSeguradoValidatorTests
{
    [Theory(DisplayName = "CNPJ numérico com máscara é aceito")]
    [InlineData("06.570.588/0001-03", "06570588000103")]
    [InlineData("06570588000103", "06570588000103")]
    public void ValidarENormalizar_CnpjNumerico_RetornaSemMascara(string entrada, string esperado)
    {
        var resultado = DocumentoSeguradoValidator.ValidarENormalizar(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory(DisplayName = "CNPJ alfanumérico com máscara é aceito")]
    [InlineData("12.ABC.345/01DE-35", "12ABC34501DE35")]
    [InlineData("8H.9XJ.6M0/0001-73", "8H9XJ6M0000173")]
    public void ValidarENormalizar_CnpjAlfanumerico_RetornaSemMascara(string entrada, string esperado)
    {
        var resultado = DocumentoSeguradoValidator.ValidarENormalizar(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory(DisplayName = "CPF com máscara é aceito")]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("52998224725", "52998224725")]
    public void ValidarENormalizar_Cpf_RetornaSomenteDigitos(string entrada, string esperado)
    {
        var resultado = DocumentoSeguradoValidator.ValidarENormalizar(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Fact(DisplayName = "Rejeita CNPJ alfanumérico com dígito verificador inválido")]
    public void ValidarENormalizar_CnpjAlfanumericoDvInvalido_LancaExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => DocumentoSeguradoValidator.ValidarENormalizar("8H.9XJ.6M0/0001-99"));

        Assert.Contains("CNPJ inválido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Rejeita CNPJ numérico com dígito verificador inválido")]
    public void ValidarENormalizar_CnpjNumericoDvInvalido_LancaExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => DocumentoSeguradoValidator.ValidarENormalizar("06.570.588/0001-99"));

        Assert.Contains("CNPJ inválido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
