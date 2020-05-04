namespace API.Entities
{
    public enum ErrosEnumeration
    {
        EmailJaExiste = 1,
        NumTelemovelJaExiste = 2,
        NomeInvalido = 3,
        EmailInvalido = 4,
        PasswordInvalida = 5,
        NumTelemovelInvalido = 6,
        EmailNaoExiste = 7,
        CodigoValidacaoErrado = 8,
        NumTentativasExcedido = 9,
        ContaNaoConfirmada = 10,
        ContaJaConfirmada = 11,
        ContaNaoExiste = 12,
        EmailPasswordIncorreta = 13,
        PasswordsNaoCorrespondem = 14,


        NumFuncionarioJaExiste = 20,
        NumFuncionarioNaoExiste = 21,
        NumFuncionarioInvalido = 22,

        NomeProdutoJaExiste = 30,
        PrecoInvalido = 31,
        ProdutoNaoExiste = 32,
        ProdutoDesativado = 33,
        ProdutoAtivado = 34,

        CategoriaNaoExiste = 40,
        NomeCategoriaJaExiste = 41,

        FormatoImagemInvalido = 50,
        ImagemVazia = 51,

        
    }
}
