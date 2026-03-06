using System;

namespace BuscaPromocao.Application.Features.Promocoes.DTOs;

public sealed record PromocaoDto(
    string Perfil, 
    string Texto, 
    string UrlTweet, 
    DateTime DataPublicacao
);
