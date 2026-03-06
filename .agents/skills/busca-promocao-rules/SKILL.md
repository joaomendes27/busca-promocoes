---
name: busca-promocao-rules
description: Regras de qualidade de código, convenções de nomenclatura, arquitetura e padrões do projeto Busca Promoção. Esta skill deve ser consultada SEMPRE que for criar ou modificar código neste projeto.
---

# Busca Promoção - Regras do Projeto

Esta skill define as regras e convenções obrigatórias do projeto **Busca Promoção**. Consulte sempre antes de criar ou modificar código.

## Quando Usar Esta Skill

Use esta skill **SEMPRE** que:

- For criar novos arquivos (classes, controllers, handlers, etc.)
- For refatorar código existente
- For adicionar novas funcionalidades
- For escrever testes
- Tiver dúvida sobre nomenclatura ou padrões

---

## Arquitetura

O projeto segue **Clean Architecture** com os seguintes layers:

| Layer | Projeto | Responsabilidade |
|-------|---------|-----------------|
| Domain | `BuscaPromocao.Domain` | Entidades, interfaces de repositório, value objects |
| Application | `BuscaPromocao.Application` | Commands, Queries (CQRS via MediatR), Handlers, DTOs |
| Infrastructure | `BuscaPromocao.Infrastructure` | EF Core, repositórios, serviços externos, Redis, RabbitMQ |
| Presentation | `BuscaPromocao.Api` | Controllers, middleware, configuração |
| Workers | `BuscaPromocao.Crawler` / `BuscaPromocao.Notifier` | Background services |

### Stack Tecnológica

- **Backend:** .NET 8, Entity Framework Core 8, MediatR (CQRS)
- **Frontend:** Angular 17+
- **Banco de Dados:** PostgreSQL
- **Cache/Deduplicação:** Redis
- **Mensageria:** RabbitMQ (MassTransit)
- **RSS:** Nitter (Docker self-hosted)
- **Autenticação:** JWT Bearer Tokens

---

## Padrões de Qualidade de Código

> [!IMPORTANT]
> Estas regras são **obrigatórias** em todo código novo ou refatorado.

### 1. Código Limpo e Sem Comentários

- O código deve ser **autoexplicativo**
- Nomes de variáveis, métodos e classes devem comunicar a intenção **sem necessidade de comentários**
- Não use comentários explicativos — se o código precisa de comentário, ele precisa ser reescrito

### 2. Nomenclatura em Português

- **Todas** as classes, propriedades, DTOs, Commands, Queries e nomes de tabelas/colunas devem estar em **português**
- **Banco de dados:** usar `snake_case` para tabelas e colunas (ex: `palavras_chave`, `data_criacao`)
- **C#:** usar `PascalCase` para classes e métodos, `camelCase` para variáveis locais

#### Exemplos de Nomenclatura

```csharp
// ✅ CORRETO
public class CriarPalavraChaveCommand : IRequest<Guid> { }
public class ObterNotificacoesPorUsuarioQuery : IRequest<List<NotificacaoDto>> { }
public class PalavraChave { public string Termo { get; set; } }

// ❌ ERRADO
public class CreateKeywordCommand : IRequest<Guid> { }
public class GetNotificationsByUserQuery : IRequest<List<NotificationDto>> { }
public class Keyword { public string Term { get; set; } }
```

### 3. Organização de Pastas (Application Layer)

Seguir o padrão por funcionalidade:

```
BuscaPromocao.Application/
├── Features/
│   ├── PalavrasChave/
│   │   ├── Commands/
│   │   │   ├── CriarPalavraChave/
│   │   │   │   ├── CriarPalavraChaveCommand.cs
│   │   │   │   └── CriarPalavraChaveCommandHandler.cs
│   │   │   └── RemoverPalavraChave/
│   │   │       ├── RemoverPalavraChaveCommand.cs
│   │   │       └── RemoverPalavraChaveCommandHandler.cs
│   │   └── Queries/
│   │       └── ObterPalavrasChavePorUsuario/
│   │           ├── ObterPalavrasChavePorUsuarioQuery.cs
│   │           └── ObterPalavrasChavePorUsuarioQueryHandler.cs
│   ├── Notificacoes/
│   ├── Perfis/
│   └── Usuarios/
```

### 4. Testes Contínuos

- **Toda** nova funcionalidade **deve** ter testes unitários
- Framework: **xUnit** + **Moq** + **FluentAssertions**
- Cobrir **cenários de sucesso e falha**
- Nomenclatura de testes: `MetodoTestado_Cenario_ResultadoEsperado`

```csharp
// ✅ Exemplo de teste
[Fact]
public async Task Handle_QuandoPalavraChaveValida_DeveCriarComSucesso()
{
    // Arrange
    var comando = new CriarPalavraChaveCommand("Air Fryer", usuarioId);
    _repositorioMock.Setup(r => r.AdicionarAsync(It.IsAny<PalavraChave>()))
        .Returns(Task.CompletedTask);

    // Act
    var resultado = await _handler.Handle(comando, CancellationToken.None);

    // Assert
    resultado.Should().NotBeEmpty();
    _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<PalavraChave>()), Times.Once);
}
```

### 5. Boas Práticas

- Seguir **SOLID**, **Clean Architecture** e **DRY**
- Métodos **pequenos** e com **responsabilidade única**
- Usar **injeção de dependência** sempre
- Usar **async/await** para operações I/O
- Validações em **Commands** usando FluentValidation ou validações manuais
- Retornar **Result pattern** ou exceções tipadas para tratamento de erros

### 6. Entity Framework (Banco de Dados)

- Configurações de entidade em classes separadas (`IEntityTypeConfiguration<T>`)
- Usar **snake_case** no mapeamento de tabelas e colunas
- Migrations com nomes descritivos em português

```csharp
// ✅ Exemplo de mapeamento
public class PalavraChaveConfiguration : IEntityTypeConfiguration<PalavraChave>
{
    public void Configure(EntityTypeBuilder<PalavraChave> builder)
    {
        builder.ToTable("palavras_chave");
        builder.Property(p => p.Termo).HasColumnName("termo").IsRequired();
        builder.Property(p => p.DataCriacao).HasColumnName("data_criacao");
    }
}
```

### 7. Padrões de Commit

- Os **títulos dos commits** devem ser **autoexplicativos** e sempre em **português**
- Utilize o padrão Conventional Commits (ex: `feat:`, `fix:`, `refactor:`, `chore:`)
- **Exemplo correto:** `feat(auth): implementa autenticação JWT completa com endpoints de login e registro`
- **Exemplo incorreto:** `fixed stuff` ou `add jwt`

---

## Checklist Antes de Submeter Código

- [ ] Nomes de classes/métodos/propriedades em português?
- [ ] Sem comentários desnecessários?
- [ ] Testes unitários criados para cenários de sucesso e falha?
- [ ] Mapeamento de banco de dados usando snake_case?
- [ ] Código segue Clean Architecture (Domain não depende de Infrastructure)?
- [ ] Métodos pequenos com responsabilidade única?
