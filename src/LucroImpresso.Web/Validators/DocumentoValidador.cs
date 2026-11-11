namespace LucroImpresso.Web.Validators;

/// <summary>
/// RF022: validação de integridade de CPF e CNPJ pelos dígitos verificadores (módulo 11).
/// Aceita os documentos com ou sem máscara (pontos, traço e barra são ignorados).
/// </summary>
public static class DocumentoValidador
{
    public static string SomenteDigitos(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsDigit).ToArray());

    public static bool CpfValido(string? cpf)
    {
        var d = SomenteDigitos(cpf);
        if (d.Length != 11 || d.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(d[..9], [10, 9, 8, 7, 6, 5, 4, 3, 2]) == d[9] - '0'
            && DigitoVerificador(d[..10], [11, 10, 9, 8, 7, 6, 5, 4, 3, 2]) == d[10] - '0';
    }

    public static bool CnpjValido(string? cnpj)
    {
        var d = SomenteDigitos(cnpj);
        if (d.Length != 14 || d.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(d[..12], [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) == d[12] - '0'
            && DigitoVerificador(d[..13], [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) == d[13] - '0';
    }

    private static int DigitoVerificador(string base_, int[] pesos)
    {
        var soma = base_.Select((c, i) => (c - '0') * pesos[i]).Sum();
        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
